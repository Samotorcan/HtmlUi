/// <reference path="references.ts" />
// definitions
var htmlUi;
(function (htmlUi) {
    var angular;
    (function (angular) {
        var ControllerChange = (function () {
            function ControllerChange(controllerId) {
                this.id = controllerId;
                this.properties = {};
                this.observableCollections = {};
            }
            Object.defineProperty(ControllerChange.prototype, "hasChanges", {
                get: function () {
                    return htmlUi._.keys(this.properties).length != 0 ||
                        (htmlUi._.keys(this.observableCollections).length != 0 && htmlUi._.any(this.observableCollections, function (changes) { return changes.hasChanges; }));
                },
                enumerable: true,
                configurable: true
            });
            ControllerChange.prototype.getObservableCollection = function (propertyName) {
                var observableCollections = htmlUi._.find(this.observableCollections, function (observableCollection) { observableCollection.name == propertyName; });
                if (observableCollections == null) {
                    observableCollections = new ObservableCollectionChanges();
                    this.observableCollections[propertyName] = observableCollections;
                }
                return observableCollections;
            };
            ControllerChange.prototype.setProperty = function (propertyName, value) {
                this.properties[propertyName] = value;
            };
            ControllerChange.prototype.getProperty = function (propertyName) {
                return this.properties[propertyName];
            };
            ControllerChange.prototype.hasProperty = function (propertyName) {
                return htmlUi._.has(this.properties, propertyName);
            };
            ControllerChange.prototype.removeProperty = function (propertyName) {
                if (this.hasProperty(propertyName))
                    delete this.properties[propertyName];
            };
            ControllerChange.prototype.addObservableCollectionChange = function (propertyName, action, newItem, newStartingIndex, oldStartingIndex) {
                this.getObservableCollection(propertyName).actions
                    .push(new ObservableCollectionChange(action, newItem, newStartingIndex, oldStartingIndex));
            };
            ControllerChange.prototype.hasObservableCollection = function (propertyName) {
                return htmlUi._.has(this.observableCollections, propertyName);
            };
            ControllerChange.prototype.removeObservableCollection = function (propertyName) {
                if (this.hasObservableCollection(propertyName))
                    delete this.observableCollections[propertyName];
            };
            ControllerChange.prototype.clear = function () {
                this.properties = {};
                this.observableCollections = {};
            };
            return ControllerChange;
        })();
        angular.ControllerChange = ControllerChange;
        var ObservableCollectionChanges = (function () {
            function ObservableCollectionChanges() {
                this.actions = [];
            }
            Object.defineProperty(ObservableCollectionChanges.prototype, "hasChanges", {
                get: function () {
                    return htmlUi._.keys(this.actions).length != 0;
                },
                enumerable: true,
                configurable: true
            });
            return ObservableCollectionChanges;
        })();
        angular.ObservableCollectionChanges = ObservableCollectionChanges;
        var ObservableCollectionChange = (function () {
            function ObservableCollectionChange(action, newItem, newStartingIndex, oldStartingIndex) {
                this.action = action;
                this.newItems = [newItem];
                this.newStartingIndex = newStartingIndex;
                this.oldStartingIndex = oldStartingIndex;
                if (this.newItems == null)
                    this.newItems = [];
            }
            return ObservableCollectionChange;
        })();
        angular.ObservableCollectionChange = ObservableCollectionChange;
        var ControllerDataContainer = (function () {
            function ControllerDataContainer() {
                this.data = {};
                this.controllerChanges = [];
            }
            Object.defineProperty(ControllerDataContainer.prototype, "hasControllerChanges", {
                get: function () {
                    return htmlUi._.any(this.controllerChanges, function (controllerChange) { return controllerChange.hasChanges; });
                },
                enumerable: true,
                configurable: true
            });
            ControllerDataContainer.prototype.addClientControllerData = function (controllerId) {
                var controllerData = null;
                this.data[controllerId] = controllerData = new ControllerData(controllerId);
                return controllerData;
            };
            ControllerDataContainer.prototype.addControllerData = function (controllerId) {
                var controllerData = this.addClientControllerData(controllerId);
                this.controllerChanges.push(controllerData.change);
                return controllerData;
            };
            ControllerDataContainer.prototype.getControllerData = function (controllerId) {
                return this.data[controllerId];
            };
            ControllerDataContainer.prototype.getControllerDataByScopeId = function (scopeId) {
                return htmlUi._.find(this.data, function (controllerData) {
                    return controllerData.scopeId == scopeId;
                });
            };
            ControllerDataContainer.prototype.clearControllerChanges = function () {
                htmlUi._.forEach(this.controllerChanges, function (controllerChange) {
                    controllerChange.clear();
                });
            };
            return ControllerDataContainer;
        })();
        angular.ControllerDataContainer = ControllerDataContainer;
        var ControllerData = (function () {
            function ControllerData(controllerId) {
                this.controllerId = controllerId;
                this.propertyValues = {};
                this.observableCollectionValues = {};
                this.watches = {};
                this.change = new ControllerChange(controllerId);
            }
            ControllerData.prototype.hasProperty = function (propertyName) {
                return htmlUi._.has(this.propertyValues, propertyName);
            };
            ControllerData.prototype.hasPropertyValue = function (propertyName, propertyValue) {
                return this.hasProperty(propertyName) && this.propertyValues[propertyName] === propertyValue;
            };
            ControllerData.prototype.setPropertyValue = function (propertyName, propertyValue) {
                this.propertyValues[propertyName] = propertyValue;
            };
            ControllerData.prototype.removePropertyValue = function (propertyName) {
                if (this.hasProperty(propertyName))
                    delete this.propertyValues[propertyName];
            };
            ControllerData.prototype.hasObservableCollection = function (propertyName) {
                return htmlUi._.has(this.observableCollectionValues, propertyName);
            };
            ControllerData.prototype.hasObservableCollectionValue = function (propertyName, observableCollectionValue) {
                return this.hasObservableCollection(propertyName) && htmlUi.utility.isArrayShallowEqual(this.observableCollectionValues[propertyName], observableCollectionValue);
            };
            ControllerData.prototype.setObservableCollectionValue = function (propertyName, observableCollectionValue) {
                this.observableCollectionValues[propertyName] = observableCollectionValue;
            };
            ControllerData.prototype.removeObservableCollectionValue = function (propertyName) {
                if (this.hasObservableCollection(propertyName))
                    delete this.observableCollectionValues[propertyName];
            };
            ControllerData.prototype.setControllerPropertyValue = function (propertyName, propertyValue) {
                this.$scope[propertyName] = propertyValue;
            };
            ControllerData.prototype.hasWatch = function (propertyName) {
                return htmlUi._.has(this.watches, propertyName);
            };
            ControllerData.prototype.removeWatch = function (propertyName) {
                if (this.hasWatch(propertyName)) {
                    this.watches[propertyName]();
                    delete this.watches[propertyName];
                }
            };
            ControllerData.prototype.addWatch = function (propertyName, watch) {
                this.removeWatch(propertyName);
                this.watches[propertyName] = watch;
            };
            return ControllerData;
        })();
        angular.ControllerData = ControllerData;
    })(angular = htmlUi.angular || (htmlUi.angular = {}));
})(htmlUi || (htmlUi = {}));
// services
var htmlUi;
(function (htmlUi) {
    var angular;
    (function (angular) {
        var _angular = window['angular'];
        var _$q = null;
        angular.services = {
            get $q() {
                if (_$q == null)
                    _$q = _angular.injector(['ng']).get('$q');
                return _$q;
            }
        };
    })(angular = htmlUi.angular || (htmlUi.angular = {}));
})(htmlUi || (htmlUi = {}));
// native
var htmlUi;
(function (htmlUi) {
    var angular;
    (function (angular) {
        var native;
        (function (native) {
            function syncControllerChanges(controllerChanges) {
                htmlUi.native.callNativeSync('syncControllerChanges', controllerChanges);
            }
            native.syncControllerChanges = syncControllerChanges;
            function syncControllerChangesAsync(controllerChanges) {
                return callNativeAsync('syncControllerChanges', controllerChanges);
            }
            native.syncControllerChangesAsync = syncControllerChangesAsync;
            function getControllerNames() {
                return htmlUi.native.callNativeSync('getControllerNames');
            }
            native.getControllerNames = getControllerNames;
            function getControllerNamesAsync() {
                return callNativeAsync('getControllerNames', null);
            }
            native.getControllerNamesAsync = getControllerNamesAsync;
            function createController(name) {
                return htmlUi.native.callNativeSync('createController', { name: name });
            }
            native.createController = createController;
            function createControllerAsync(name) {
                return callNativeAsync('createController', { name: name });
            }
            native.createControllerAsync = createControllerAsync;
            function createObservableController(name) {
                return htmlUi.native.callNativeSync('createObservableController', { name: name });
            }
            native.createObservableController = createObservableController;
            function createObservableControllerAsync(name) {
                return callNativeAsync('createObservableController', { name: name });
            }
            native.createObservableControllerAsync = createObservableControllerAsync;
            function destroyController(id) {
                htmlUi.native.callNativeSync('destroyController', id);
            }
            native.destroyController = destroyController;
            function destroyControllerAsync(id) {
                return callNativeAsync('destroyController', id);
            }
            native.destroyControllerAsync = destroyControllerAsync;
            function callMethod(id, name, args) {
                return htmlUi.native.callNativeSync('callMethod', { id: id, name: name, args: args });
            }
            native.callMethod = callMethod;
            function callMethodAsync(id, name, args) {
                return callNativeAsync('callMethod', { id: id, name: name, args: args });
            }
            native.callMethodAsync = callMethodAsync;
            function callInternalMethod(id, name, args) {
                return htmlUi.native.callNativeSync('callMethod', { id: id, name: name, args: args, internalMethod: true });
            }
            native.callInternalMethod = callInternalMethod;
            function callInternalMethodAsync(id, name, args) {
                return callNativeAsync('callMethod', { id: id, name: name, args: args, internalMethod: true });
            }
            native.callInternalMethodAsync = callInternalMethodAsync;
            function registerFunction(name, func) {
                htmlUi.native.registerFunction(name, func);
            }
            native.registerFunction = registerFunction;
            function log(type, messageType, message) {
                htmlUi.native.callNativeSync('log', { type: type, messageType: messageType, message: message });
            }
            native.log = log;
            function callNativeAsync(name, data) {
                var deferred = angular.services.$q.defer();
                htmlUi.native.callNativeAsync(name, data, function (response) {
                    if (response.type == htmlUi.NativeResponseType.Value)
                        deferred.resolve(response.value);
                    else if (response.type == htmlUi.NativeResponseType.Exception)
                        deferred.reject(response.exception);
                    else
                        deferred.resolve();
                });
                return deferred.promise;
            }
        })(native = angular.native || (angular.native = {}));
    })(angular = htmlUi.angular || (htmlUi.angular = {}));
})(htmlUi || (htmlUi = {}));
// main
var htmlUi;
(function (htmlUi) {
    var angular;
    (function (angular) {
        var _angular = window['angular'];
        var _controllerDataContainer = new angular.ControllerDataContainer();
        init();
        function init() {
            // register functions
            angular.native.registerFunction('syncControllerChanges', syncControllerChanges);
            angular.native.registerFunction('callClientFunction', callClientFunction);
            // module
            var htmlUiModule = _angular.module('htmlUi', []);
            // run
            htmlUiModule.run(['$rootScope', function ($rootScope) {
                    $rootScope['htmlUiControllerChanges'] = _controllerDataContainer.controllerChanges;
                    addHtmlUiControllerChangesWatch($rootScope);
                }]);
            // controller service
            htmlUiModule.factory('htmlUi.controller', [function () {
                    var createController = function (controllerName) {
                        // create controller
                        var controller = angular.native.createController(controllerName);
                        var controllerData = _controllerDataContainer.addClientControllerData(controller.id);
                        controllerData.name = controllerName;
                        var clientController = controllerData.clientController = {
                            destroy: function () {
                                angular.native.destroyController(controller.id);
                            }
                        };
                        // methods
                        htmlUi._.forEach(controller.methods, function (method) {
                            clientController[method.name] = function () {
                                return angular.native.callMethod(controller.id, method.name, htmlUi.utility.argumentsToArray(arguments));
                            };
                        });
                        // warm up native calls
                        angular.native.callInternalMethodAsync(controller.id, 'warmUp', ['warmUp']).then(function () { });
                        return clientController;
                    };
                    var createObservableController = function (controllerName, $scope) {
                        var scopeId = $scope.$id;
                        // create observable controller
                        var observableController = angular.native.createObservableController(controllerName);
                        var controllerData = _controllerDataContainer.addControllerData(observableController.id);
                        controllerData.name = controllerName;
                        controllerData.$scope = $scope;
                        controllerData.scopeId = $scope.$id;
                        // properties
                        htmlUi._.forEach(observableController.properties, function (property) {
                            var propertyName = property.name;
                            $scope[propertyName] = property.value;
                            // watch observable collection
                            if (htmlUi._.isArray(property.value))
                                addCollectionWatch(propertyName, $scope);
                            // watch property
                            addPropertyWatch(propertyName, $scope);
                        });
                        // methods
                        htmlUi._.forEach(observableController.methods, function (method) {
                            $scope[method.name] = function () {
                                return angular.native.callMethod(observableController.id, method.name, htmlUi.utility.argumentsToArray(arguments));
                            };
                        });
                        // destroy controller
                        $scope.$on('$destroy', function () {
                            angular.native.destroyController(observableController.id);
                        });
                        // warm up native calls
                        angular.native.callInternalMethodAsync($scope.$id, 'warmUp', ['warmUp']).then(function () { });
                        return $scope;
                    };
                    return {
                        createController: createController,
                        createObservableController: createObservableController
                    };
                }]);
        }
        function addHtmlUiControllerChangesWatch($rootScope) {
            $rootScope.$watch('htmlUiControllerChanges', function () {
                if (!_controllerDataContainer.hasControllerChanges)
                    return;
                try {
                    angular.native.syncControllerChanges(_controllerDataContainer.controllerChanges);
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
                    if (htmlUi._.isArray(oldValue))
                        removeCollectionWatch(propertyName, $scope);
                    if (htmlUi._.isArray(newValue))
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
                if (newCollection !== oldCollection && !htmlUi.utility.isArrayShallowEqual(newCollection, oldCollection) &&
                    !controllerData.hasObservableCollectionValue(propertyName, newCollection) &&
                    !controllerData.change.hasProperty(propertyName)) {
                    var compareValues = htmlUi._.zip(oldCollection, newCollection);
                    htmlUi._.forEach(compareValues, function (compareValue, index) {
                        var oldValue = compareValue[0];
                        var newValue = compareValue[1];
                        if (index < oldCollection.length && index < newCollection.length) {
                            // replace
                            if (oldValue !== newValue) {
                                controllerData.change.addObservableCollectionChange(propertyName, htmlUi.ObservableCollectionChangeAction.Replace, newValue, index, null);
                            }
                        }
                        else if (index < oldCollection.length && index >= newCollection.length) {
                            // remove
                            controllerData.change.addObservableCollectionChange(propertyName, htmlUi.ObservableCollectionChangeAction.Remove, null, null, index);
                        }
                        else {
                            // add
                            controllerData.change.addObservableCollectionChange(propertyName, htmlUi.ObservableCollectionChangeAction.Add, newValue, index, null);
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
            htmlUi._.forEach(controllerChanges, function (controllerChange) {
                var controllerId = controllerChange.id;
                var controllerData = _controllerDataContainer.getControllerData(controllerId);
                var controller = controllerData.$scope;
                controller.$apply(function () {
                    // properties
                    htmlUi._.forEach(controllerChange.properties, function (value, propertyName) {
                        var propertyName = htmlUi._.camelCase(propertyName);
                        controllerData.setControllerPropertyValue(propertyName, value);
                        controllerData.setPropertyValue(propertyName, value);
                    });
                    // observable collections
                    htmlUi._.forEach(controllerChange.observableCollections, function (changes, propertyName) {
                        var propertyName = htmlUi._.camelCase(propertyName);
                        if (!htmlUi._.isArray(controller[propertyName]))
                            controller[propertyName] = [];
                        var collection = controller[propertyName];
                        htmlUi._.forEach(changes.actions, function (change) {
                            switch (change.action) {
                                case htmlUi.ObservableCollectionChangeAction.Add:
                                    observableCollectionAddAction(collection, change);
                                    break;
                                case htmlUi.ObservableCollectionChangeAction.Remove:
                                    observableCollectionRemoveAction(collection, change);
                                    break;
                                case htmlUi.ObservableCollectionChangeAction.Replace:
                                    observableCollectionReplaceAction(collection, change);
                                    break;
                                case htmlUi.ObservableCollectionChangeAction.Move:
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
            htmlUi._.forEach(insertItems, function (insertItem) {
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
            htmlUi._.forEach(replaceItems, function (replaceItem) {
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
        function callClientFunction(json) {
            var clientFunction = JSON.parse(json);
            var controllerData = _controllerDataContainer.getControllerData(clientFunction.controllerId);
            var runFunction = function (func) {
                var result = {
                    type: htmlUi.ClientFunctionResultType.Value,
                    exception: null,
                    value: null
                };
                if (htmlUi._.isFunction(func)) {
                    try {
                        result.value = func.apply({}, clientFunction.args);
                    }
                    catch (err) {
                        result.type = htmlUi.ClientFunctionResultType.Exception;
                        result.exception = err;
                    }
                    if (result.value === undefined)
                        result.type = htmlUi.ClientFunctionResultType.Undefined;
                }
                else {
                    result.type = htmlUi.ClientFunctionResultType.FunctionNotFound;
                }
                return result;
            };
            if (controllerData.clientController != null)
                return runFunction(controllerData.clientController[clientFunction.name]);
            return runFunction(controllerData.$scope[clientFunction.name]);
        }
    })(angular = htmlUi.angular || (htmlUi.angular = {}));
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=htmlUi.angular.js.map