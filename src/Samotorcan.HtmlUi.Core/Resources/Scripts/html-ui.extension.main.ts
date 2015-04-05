/// <reference path="references.ts" />

module htmlUi {
    var _controllerChanges: IControllerChanges = null;
    var _controllerNames: string[] = null;
    var _controllerPropertyValues: { [id: number]: { [name: string]: any } } = {};
    var _controllerObservableCollectionValues: { [id: number]: { [name: string]: any } } = {};
    var _controllers: { [id: number]: any } = {};
    var _arrayWatches: { [id: number]: { [name: string]: Function } } = {};

    function addHtmlUiControllerChangesWatch($rootScope: ng.IRootScopeService): void {
        $rootScope.$watch('htmlUiControllerChanges',(controllerChanges: IControllerChanges): void => {
            if (!controllerChanges.hasChanges)
                return;

            try {
                htmlUi.native.syncControllerChanges(controllerChanges.changes);
            } finally {
                _controllerChanges.clear();
            }
        }, true);
    }

    export var app = null;

    export function init() {
        _controllerNames = htmlUi.native.getControllerNames();

        // register functions
        htmlUi.native.registerFunction('syncControllerChanges', syncControllerChanges);

        // create module
        htmlUi.app = angular.module('htmlUiApp', []);

        htmlUi.app.run(['$rootScope', function ($rootScope: ng.IRootScopeService) {
            _controllerChanges = $rootScope['htmlUiControllerChanges'] = new ControllerChanges();

            addHtmlUiControllerChangesWatch($rootScope);
        }]);

        // create controllers
        _.forEach(_controllerNames, function (controllerName) {
            htmlUi.app.controller(controllerName, ['$scope', function ($scope: ng.IScope) {
                // save controller
                _controllers[$scope.$id] = $scope;

                // create controller
                var controller = htmlUi.native.createController(controllerName, $scope.$id);

                // properties
                _.forEach(controller.properties, function (property: any) {
                    var propertyName = property.name;
                    $scope[propertyName] = property.value;

                    // add array watch
                    if (_.isArray(property.value))
                        addArrayWatch(propertyName, $scope);

                    // watch property
                    $scope.$watch(propertyName, function (newValue, oldValue) {
                        var controllerPropertyValue = _controllerPropertyValues[$scope.$id] || {};
                        var hasPropertyValue = _.has(controllerPropertyValue, propertyName);

                        // ignore sync if the value is already set in the server controller
                        if (newValue !== oldValue && (!hasPropertyValue || controllerPropertyValue[propertyName] !== newValue)) {
                            var controllerChanges = _controllerChanges.getChange($scope.$id);

                            controllerChanges.properties[propertyName] = newValue;

                            if (_.isArray(oldValue))
                                removeArrayWatch(propertyName, $scope);

                            if (_.isArray(newValue))
                                addArrayWatch(propertyName, $scope);

                            if (_.has(controllerChanges.observableCollections, propertyName))
                                delete controllerChanges.observableCollections[propertyName];
                        }

                        if (hasPropertyValue)
                            delete controllerPropertyValue[propertyName];
                    });
                });

                // methods
                _.forEach(controller.methods, function (method) {
                    $scope[method.name] = function () {
                        return htmlUi.native.callMethod($scope.$id, method.name, argumentsToArray(arguments));
                    };
                });

                // destroy controller
                $scope.$on('$destroy', function () {
                    htmlUi.native.destroyController($scope.$id);
                });

                // warm up native calls
                htmlUi.native.callInternalMethodAsync($scope.$id, 'warmUp', ['warmUp']).then(function () { });
            }]);
        });

        function addArrayWatch(propertyName: string, $scope: ng.IScope) {
            var controllerArrayWatches = _arrayWatches[$scope.$id] = (_arrayWatches[$scope.$id] || {});

            // watch collection
            controllerArrayWatches[propertyName] = $scope.$watchCollection(propertyName, function (newArray, oldArray): void {
                var controllerChanges = _controllerChanges.getChange($scope.$id);

                var controllerObservableCollectionValue = _controllerObservableCollectionValues[$scope.$id] || {};
                var hasObservableCollectionValue = _.has(controllerObservableCollectionValue, propertyName);

                if (newArray !== oldArray && !isArrayShallowEqual(newArray, oldArray) &&
                    (!hasObservableCollectionValue || !isArrayShallowEqual(newArray, controllerObservableCollectionValue[propertyName])) &&
                    !_.has(controllerChanges.properties, propertyName)) {

                    var controllerObservableCollectionChanges = controllerChanges.observableCollections;
                    var observableCollectionChanges = controllerObservableCollectionChanges[propertyName] = (controllerObservableCollectionChanges[propertyName] || { name: propertyName, actions: [] });
                    var observableCollectionChangesActions = observableCollectionChanges.actions;

                    var compareValues = _.zip(oldArray, newArray);

                    _.forEach(compareValues, function (compareValue, index) {
                        var oldValue = compareValue[0];
                        var newValue = compareValue[1];

                        if (index < oldArray.length && index < newArray.length) {
                            // replace
                            if (oldValue !== newValue) {
                                observableCollectionChangesActions.push({
                                    action: ObservableCollectionChangeAction.Replace,
                                    newStartingIndex: index,
                                    newItems: [newValue]
                                });
                            }
                        } else if (index < oldArray.length && index >= newArray.length) {
                            // remove
                            observableCollectionChangesActions.push({
                                action: ObservableCollectionChangeAction.Remove,
                                oldStartingIndex: index
                            });
                        } else {
                            // add
                            observableCollectionChangesActions.push({
                                action: ObservableCollectionChangeAction.Add,
                                newStartingIndex: index,
                                newItems: [newValue]
                            });
                        }
                    });
                }

                if (hasObservableCollectionValue)
                    delete controllerObservableCollectionValue[propertyName];
            });
        }

        function removeArrayWatch(propertyName, $scope) {
            var controllerArrayWatches = _arrayWatches[$scope.$id];

            if (controllerArrayWatches != null) {
                var arrayWatch = controllerArrayWatches[propertyName];

                if (arrayWatch != null) {
                    arrayWatch();

                    delete controllerArrayWatches[propertyName];

                    if (_.keys(controllerArrayWatches).length == 0)
                        delete _arrayWatches[$scope.$id];
                }
            }
        }

        function isArrayShallowEqual(firstArray, secondArray) {
            if (firstArray === secondArray)
                return true;

            if (firstArray == null || secondArray == null)
                return false;

            if (firstArray.length != secondArray.length)
                return false;

            return !_.any(firstArray, function (value, index) {
                return value !== secondArray[index];
            });
        }

        function shallowCopyCollection(collection) {
            if (collection == null)
                return null;

            var newCollection = [];
            _.forEach(collection, function (value, index) {
                newCollection[index] = value;
            });

            return newCollection;
        }

        // sync controller changes and ignore watch values
        function syncControllerChanges(json) {
            var controllerChanges = JSON.parse(json);

            _.forEach(controllerChanges, function (controllerChange: IControllerChange) {
                var controller = _controllers[controllerChange.id];
                var controllerPropertyValue = _controllerPropertyValues[controllerChange.id] = (_controllerPropertyValues[controllerChange.id] || {});
                var controllerObservableCollectionValue = _controllerObservableCollectionValues[controllerChange.id] = (_controllerObservableCollectionValues[controllerChange.id] || {});

                controller.$apply(function () {
                    // properties
                    _.forEach(controllerChange.properties, function (value, propertyName) {
                        var propertyName = _.camelCase(propertyName);

                        controller[propertyName] = value;
                        controllerPropertyValue[propertyName] = value;
                    });

                    // observable collections
                    _.forEach(controllerChange.observableCollections, function (changes, propertyName) {
                        var propertyName = _.camelCase(propertyName);

                        if (!_.isArray(controller[propertyName]))
                            controller[propertyName] = [];

                        var array: any[] = controller[propertyName];

                        _.forEach(changes.actions, function (change: IObservableCollectionChange) {
                            switch (change.action) {
                                case ObservableCollectionChangeAction.Add:
                                    observableCollectionAddAction(array, change);
                                    break;
                                case ObservableCollectionChangeAction.Remove:
                                    observableCollectionRemoveAction(array, change);
                                    break;
                                case ObservableCollectionChangeAction.Replace:
                                    observableCollectionReplaceAction(array, change);
                                    break;
                                case ObservableCollectionChangeAction.Move:
                                    observableCollectionMoveAction(array, change);
                                    break;
                            }
                        });

                        controllerObservableCollectionValue[propertyName] = shallowCopyCollection(array);
                    });
                });
            });
        }

        function observableCollectionAddAction(array: any[], change: IObservableCollectionChange) {
            var insertIndex = change.newStartingIndex;
            var insertItems = change.newItems;

            _.forEach(insertItems, function (insertItem) {
                array.splice(insertIndex, 0, insertItem);
                insertIndex++;
            });
        }

        function observableCollectionRemoveAction(array: any[], change: IObservableCollectionChange) {
            var removeIndex = change.oldStartingIndex;

            array.splice(removeIndex, 1);
        }

        function observableCollectionReplaceAction(array: any[], change: IObservableCollectionChange) {
            var replaceIndex = change.newStartingIndex;
            var replaceItems = change.newItems;

            _.forEach(replaceItems, function (replaceItem) {
                array[replaceIndex] = replaceItem;
                replaceIndex++;
            });
        }

        function observableCollectionMoveAction(array: any[], change: IObservableCollectionChange) {
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

        function inject(func, inject) {
            return function () {
                inject.apply(this, arguments);
                return func.apply(this, arguments);
            };
        }

        function argumentsToArray(args) {
            var argsArray = [];

            _.forEach(args, function (arg) {
                argsArray.push(arg);
            });

            return argsArray;
        }
    }
} 