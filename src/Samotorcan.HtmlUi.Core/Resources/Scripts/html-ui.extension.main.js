/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    var _controllerDataContainer = new htmlUi.ControllerDataContainer();
    function init() {
        var controllerNames = htmlUi.native.getControllerNames();
        // register functions
        htmlUi.native.registerFunction('syncControllerChanges', syncControllerChanges);
        // create module
        var app = angular.module('htmlUiApp', []);
        app.run(['$rootScope', function ($rootScope) {
            $rootScope['htmlUiControllerChanges'] = _controllerDataContainer.controllerChanges;
            addHtmlUiControllerChangesWatch($rootScope);
        }]);
        // create controllers
        _.forEach(controllerNames, function (controllerName) {
            app.controller(controllerName, ['$scope', function ($scope) {
                var controllerId = $scope.$id;
                var controllerData = _controllerDataContainer.getControllerData(controllerId);
                controllerData.name = controllerName;
                controllerData.$scope = $scope;
                // create controller
                var controller = htmlUi.native.createController(controllerName, $scope.$id);
                // properties
                _.forEach(controller.properties, function (property) {
                    var propertyName = property.name;
                    $scope[propertyName] = property.value;
                    // watch observable collection
                    if (_.isArray(property.value))
                        addCollectionWatch(propertyName, $scope);
                    // watch property
                    addPropertyWatch(propertyName, $scope);
                });
                // methods
                _.forEach(controller.methods, function (method) {
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
            }]);
        });
    }
    htmlUi.init = init;
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
        var controllerId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerData(controllerId);
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
        var controllerId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerData(controllerId);
        controllerData.addWatch(propertyName, $scope.$watchCollection(propertyName, function (newArray, oldArray) {
            if (newArray !== oldArray && !htmlUi.utility.isArrayShallowEqual(newArray, oldArray) && !controllerData.hasObservableCollectionValue(propertyName, newArray) && !controllerData.change.hasProperty(propertyName)) {
                var compareValues = _.zip(oldArray, newArray);
                _.forEach(compareValues, function (compareValue, index) {
                    var oldValue = compareValue[0];
                    var newValue = compareValue[1];
                    if (index < oldArray.length && index < newArray.length) {
                        // replace
                        if (oldValue !== newValue) {
                            controllerData.change.addObservableCollectionChange(propertyName, 3 /* Replace */, newValue, index, null);
                        }
                    }
                    else if (index < oldArray.length && index >= newArray.length) {
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
        var controllerId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerData(controllerId);
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
    function observableCollectionAddAction(array, change) {
        var insertIndex = change.newStartingIndex;
        var insertItems = change.newItems;
        _.forEach(insertItems, function (insertItem) {
            array.splice(insertIndex, 0, insertItem);
            insertIndex++;
        });
    }
    function observableCollectionRemoveAction(array, change) {
        var removeIndex = change.oldStartingIndex;
        array.splice(removeIndex, 1);
    }
    function observableCollectionReplaceAction(array, change) {
        var replaceIndex = change.newStartingIndex;
        var replaceItems = change.newItems;
        _.forEach(replaceItems, function (replaceItem) {
            array[replaceIndex] = replaceItem;
            replaceIndex++;
        });
    }
    function observableCollectionMoveAction(array, change) {
        var fromIndex = change.oldStartingIndex;
        var toIndex = change.newStartingIndex;
        if (fromIndex == toIndex)
            return;
        var removedItems = array.splice(fromIndex, 1);
        if (removedItems.length == 1) {
            var removedItem = removedItems[0];
            array.splice(toIndex, 0, removedItem);
        }
    }
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.main.js.map