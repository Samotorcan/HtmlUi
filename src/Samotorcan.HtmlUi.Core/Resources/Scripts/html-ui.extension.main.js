/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    var _controllerDataContainer = new htmlUi.ControllerDataContainer();
    var _initialized = false;
    function init() {
        domAndScriptsReady(function () {
            if (_initialized)
                return;
            _initialized = true;
            // register functions
            htmlUi.native.registerFunction('syncControllerChanges', syncControllerChanges);
            // module
            var htmlUiModule = angular.module('htmlUi', []);
            // run
            htmlUiModule.run(['$rootScope', function ($rootScope) {
                $rootScope['htmlUiControllerChanges'] = _controllerDataContainer.controllerChanges;
                addHtmlUiControllerChangesWatch($rootScope);
            }]);
            // controller
            htmlUiModule.factory('htmlUi.controller', [function () {
                var createObservableController = function (controllerName, $scope) {
                    var scopeId = $scope.$id;
                    // create controller
                    var observableController = htmlUi.native.createObservableController(controllerName);
                    var controllerData = _controllerDataContainer.addControllerData(observableController.id);
                    controllerData.name = controllerName;
                    controllerData.$scope = $scope;
                    controllerData.scopeId = $scope.$id;
                    // properties
                    _.forEach(observableController.properties, function (property) {
                        var propertyName = property.name;
                        $scope[propertyName] = property.value;
                        // watch observable collection
                        if (_.isArray(property.value))
                            addCollectionWatch(propertyName, $scope);
                        // watch property
                        addPropertyWatch(propertyName, $scope);
                    });
                    // methods
                    _.forEach(observableController.methods, function (method) {
                        $scope[method.name] = function () {
                            return htmlUi.native.callMethod($scope.$id, method.name, htmlUi.utility.argumentsToArray(arguments));
                        };
                    });
                    // destroy controller
                    $scope.$on('$destroy', function () {
                        htmlUi.native.destroyController($scope.$id);
                    });
                    // warm up native calls
                    htmlUi.native.callInternalMethodAsync($scope.$id, 'warmUp', ['warmUp']).then(function () {
                    });
                    return $scope;
                };
                return {
                    createObservableController: createObservableController
                };
            }]);
            // inject htmlUi module
            if (angular['resumeBootstrap'] == null) {
                angular['resumeDeferredBootstrap'] = function () {
                    angular['resumeBootstrap']([htmlUiModule.name]);
                };
            }
            else {
                angular['resumeBootstrap']([htmlUiModule.name]);
            }
        });
    }
    htmlUi.init = init;
    function domAndScriptsReady(func) {
        domReady(function () {
            ensureScripts(func);
        });
    }
    function domReady(func) {
        if (document.readyState === 'complete')
            func();
        else
            document.addEventListener("DOMContentLoaded", func);
    }
    function ensureScripts(onload) {
        var onloadFunctions = {};
        var loadScriptInternal = function (scriptName) {
            var onloadInternal = function () {
                delete onloadFunctions[scriptName];
                if (Object.keys(onloadFunctions).length == 0 && onload != null)
                    onload();
            };
            onloadFunctions[scriptName] = onloadInternal;
            loadScript(scriptName, onloadInternal);
        };
        if (window['angular'] == null)
            loadScriptInternal('/Scripts/angular.js');
        if (window['_'] == null)
            loadScriptInternal('/Scripts/lodash.js');
        if (Object.keys(onloadFunctions).length == 0 && onload != null)
            onload();
    }
    function loadScript(scriptName, onload) {
        var scriptElement = document.createElement('script');
        document.body.appendChild(scriptElement);
        if (onload != null)
            scriptElement.onload = onload;
        scriptElement.src = scriptName;
    }
    function addHtmlUiControllerChangesWatch($rootScope) {
        $rootScope.$watch('htmlUiControllerChanges', function () {
            if (!_controllerDataContainer.hasControllerChanges)
                return;
            try {
                htmlUi.native.syncControllerChanges(_controllerDataContainer.controllerChanges);
            }
            finally {
                _controllerDataContainer.clearControllerChanges();
            }
        }, true);
    }
    function addPropertyWatch(propertyName, $scope) {
        var scopeId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerDataByScopeId(scopeId);
        $scope.$watch(propertyName, function (newValue, oldValue) {
            if (newValue !== oldValue && !controllerData.hasPropertyValue(propertyName, newValue)) {
                controllerData.change.setProperty(propertyName, newValue);
                if (_.isArray(oldValue))
                    removeCollectionWatch(propertyName, $scope);
                if (_.isArray(newValue))
                    addCollectionWatch(propertyName, $scope);
                controllerData.change.removeObservableCollection(propertyName);
            }
            controllerData.removePropertyValue(propertyName);
        });
    }
    function addCollectionWatch(propertyName, $scope) {
        var scopeId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerDataByScopeId(scopeId);
        controllerData.addWatch(propertyName, $scope.$watchCollection(propertyName, function (newCollection, oldCollection) {
            if (newCollection !== oldCollection && !htmlUi.utility.isArrayShallowEqual(newCollection, oldCollection) && !controllerData.hasObservableCollectionValue(propertyName, newCollection) && !controllerData.change.hasProperty(propertyName)) {
                var compareValues = _.zip(oldCollection, newCollection);
                _.forEach(compareValues, function (compareValue, index) {
                    var oldValue = compareValue[0];
                    var newValue = compareValue[1];
                    if (index < oldCollection.length && index < newCollection.length) {
                        // replace
                        if (oldValue !== newValue) {
                            controllerData.change.addObservableCollectionChange(propertyName, 3 /* Replace */, newValue, index, null);
                        }
                    }
                    else if (index < oldCollection.length && index >= newCollection.length) {
                        // remove
                        controllerData.change.addObservableCollectionChange(propertyName, 2 /* Remove */, null, null, index);
                    }
                    else {
                        // add
                        controllerData.change.addObservableCollectionChange(propertyName, 1 /* Add */, newValue, index, null);
                    }
                });
            }
            controllerData.removeObservableCollectionValue(propertyName);
        }));
    }
    function removeCollectionWatch(propertyName, $scope) {
        var scopeId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerDataByScopeId(scopeId);
        controllerData.removeWatch(propertyName);
    }
    function syncControllerChanges(json) {
        var controllerChanges = JSON.parse(json);
        _.forEach(controllerChanges, function (controllerChange) {
            var controllerId = controllerChange.id;
            var controllerData = _controllerDataContainer.getControllerData(controllerId);
            var controller = controllerData.$scope;
            controller.$apply(function () {
                // properties
                _.forEach(controllerChange.properties, function (value, propertyName) {
                    var propertyName = _.camelCase(propertyName);
                    controllerData.setControllerPropertyValue(propertyName, value);
                    controllerData.setPropertyValue(propertyName, value);
                });
                // observable collections
                _.forEach(controllerChange.observableCollections, function (changes, propertyName) {
                    var propertyName = _.camelCase(propertyName);
                    if (!_.isArray(controller[propertyName]))
                        controller[propertyName] = [];
                    var collection = controller[propertyName];
                    _.forEach(changes.actions, function (change) {
                        switch (change.action) {
                            case 1 /* Add */:
                                observableCollectionAddAction(collection, change);
                                break;
                            case 2 /* Remove */:
                                observableCollectionRemoveAction(collection, change);
                                break;
                            case 3 /* Replace */:
                                observableCollectionReplaceAction(collection, change);
                                break;
                            case 4 /* Move */:
                                observableCollectionMoveAction(collection, change);
                                break;
                        }
                    });
                    controllerData.setObservableCollectionValue(propertyName, htmlUi.utility.shallowCopyCollection(collection));
                });
            });
        });
    }
    function observableCollectionAddAction(collection, change) {
        var insertIndex = change.newStartingIndex;
        var insertItems = change.newItems;
        _.forEach(insertItems, function (insertItem) {
            collection.splice(insertIndex, 0, insertItem);
            insertIndex++;
        });
    }
    function observableCollectionRemoveAction(collection, change) {
        var removeIndex = change.oldStartingIndex;
        collection.splice(removeIndex, 1);
    }
    function observableCollectionReplaceAction(collection, change) {
        var replaceIndex = change.newStartingIndex;
        var replaceItems = change.newItems;
        _.forEach(replaceItems, function (replaceItem) {
            collection[replaceIndex] = replaceItem;
            replaceIndex++;
        });
    }
    function observableCollectionMoveAction(collection, change) {
        var fromIndex = change.oldStartingIndex;
        var toIndex = change.newStartingIndex;
        if (fromIndex == toIndex)
            return;
        var removedItems = collection.splice(fromIndex, 1);
        if (removedItems.length == 1) {
            var removedItem = removedItems[0];
            collection.splice(toIndex, 0, removedItem);
        }
    }
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.main.js.map