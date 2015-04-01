// TODO: clean on controller destruction

var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    htmlUi.init = function () {
        // register functions
        htmlUi._native.registerFunction('syncControllerChanges', syncControllerChanges);

        // create module
        htmlUi.app = angular.module('htmlUiApp', []);

        // inject controller creation
        htmlUi.app.controller = inject(htmlUi.app.controller, function (name) {
            
        });

        // sync controller changes
        htmlUi.app.run(['$rootScope', function ($rootScope) {
            $rootScope.htmlUiChanges = {};

            // changes
            $rootScope.$watch('htmlUiChanges', function (changes) {
                var controllerChanges = [];

                // prepare changes
                _.forEach(changes, function (properties, controllerId) {
                    if (properties != null && _.keys(properties).length > 0)
                        controllerChanges.push({ id: controllerId, properties: properties });
                });

                // call sync controller changes
                if (controllerChanges.length > 0) {
                    try {
                        htmlUi._native.syncControllerChanges(controllerChanges);
                    } finally {
                        $rootScope.htmlUiChanges = {};
                    }
                }
            }, true);
        }]);

        // create controllers
        var controllerNames = htmlUi._native.getControllerNames();
        var controllers = {};
        var arrayWatches = {};

        _.forEach(controllerNames, function (controllerName) {
            htmlUi.app.controller(controllerName, ['$scope', '$rootScope', function ($scope, $rootScope) {
                // save controller
                controllers[$scope.$id] = $scope;

                // create controller
                var controller = htmlUi._native.createController(controllerName, $scope.$id);

                // properties
                _.forEach(controller.properties, function (property) {
                    var propertyName = property.name;
                    $scope[propertyName] = property.value;

                    // add array watch
                    if (_.isArray(property.value))
                        addArrayWatch(propertyName, $scope);

                    // watch property
                    $scope.$watch(propertyName, function (newValue, oldValue) {
                        var values = controllerValues[$scope.$id] || {};
                        var hasValue = _.has(values, propertyName);

                        // ignore sync if the value is already set in the server controller
                        if (newValue !== oldValue && (!hasValue || values[propertyName] !== newValue)) {
                            var changeProperties = $rootScope.htmlUiChanges[$scope.$id] = ($rootScope.htmlUiChanges[$scope.$id] || {});

                            changeProperties[propertyName] = newValue;

                            if (_.isArray(oldValue))
                                removeArrayWatch(propertyName, $scope);

                            if (_.isArray(newValue))
                                addArrayWatch(propertyName, $scope);
                        }

                        // clear value
                        if (hasValue)
                            delete values[propertyName];
                    });
                });

                // methods
                _.forEach(controller.methods, function (method) {
                    $scope[method.name] = function () {
                        return htmlUi._native.callMethod($scope.$id, method.name, argumentsToArray(arguments));
                    };
                });

                // destroy controller
                $scope.$on('$destroy', function () {
                    htmlUi._native.destroyController($scope.$id);
                });

                // warm up native calls
                htmlUi._native.callInternalMethodAsync($scope.$id, 'warmUp', ['warmUp']).then(function () { });
            }]);
        });

        function addArrayWatch(propertyName, $scope) {
            var controllerArrayWatches = arrayWatches[$scope.$id] = (arrayWatches[$scope.$id] || {});

            controllerArrayWatches[propertyName] = $scope.$watchCollection(propertyName, function (newArray, oldArray) {
                if (newArray == oldArray || isArrayShallowEqual(newArray, oldArray))
                    return;

                console.log(newArray);
            });
        }

        function removeArrayWatch(propertyName, $scope) {
            var controllerArrayWatches = arrayWatches[$scope.$id];

            if (controllerArrayWatches != null) {
                var arrayWatch = controllerArrayWatches[propertyName];

                if (arrayWatch != null) {
                    arrayWatch();

                    delete controllerArrayWatches[propertyName];

                    if (_.keys(controllerArrayWatches).length == 0)
                        delete arrayWatches[$scope.$id];
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

        // sync controller changes and ignore watch values
        var controllerValues = {};
        function syncControllerChanges(json) {
            var controllerChanges = JSON.parse(json);

            _.forEach(controllerChanges, function (controllerChange) {
                var controller = controllers[controllerChange.Id];
                var values = controllerValues[controllerChange.Id] = (controllerValues[controllerChange.Id] || {});

                controller.$apply(function () {
                    // properties
                    _.forEach(controllerChange.Properties, function (value, propertyName) {
                        var propertyName = _.camelCase(propertyName);

                        controller[propertyName] = value;
                        values[propertyName] = value;
                    });

                    // observable collections
                    _.forEach(controllerChange.ObservableCollections, function (changes, propertyName) {
                        var propertyName = _.camelCase(propertyName);

                        if (!_.isArray(controller[propertyName]))
                            controller[propertyName] = [];

                        var array = controller[propertyName];

                        _.forEach(changes.Actions, function (change) {
                            switch (change.Action) {
                                case 'Add':
                                    ObservableCollectionAddAction(array, change);
                                    break;
                                case 'Remove':
                                    ObservableCollectionRemoveAction(array, change);
                                    break;
                                case 'Replace':
                                    ObservableCollectionReplaceAction(array, change);
                                    break;
                                case 'Move':
                                    ObservableCollectionMoveAction(array, change);
                                    break;
                            }
                        });
                    });
                });
            });
        }

        function ObservableCollectionAddAction(array, change) {
            var insertIndex = change.NewStartingIndex;
            var insertItems = change.NewItems;

            _.forEach(insertItems, function (insertItem) {
                array.splice(insertIndex, 0, insertItem);
                insertIndex++;
            });
        }

        function ObservableCollectionRemoveAction(array, change) {
            var removeIndex = change.OldStartingIndex;

            array.splice(removeIndex, 1);
        }

        function ObservableCollectionReplaceAction(array, change) {
            var replaceIndex = change.NewStartingIndex;
            var replaceItems = change.NewItems;

            _.forEach(replaceItems, function (replaceItem) {
                array[replaceIndex] = replaceItem;
                replaceIndex++;
            });
        }

        function ObservableCollectionMoveAction(array, change) {
            var fromIndex = change.OldStartingIndex;
            var toIndex = change.NewStartingIndex;

            if (fromIndex == toIndex)
                return;

            var removedItems = array.splice(fromIndex, 1);

            if (removedItems.length == 1) {
                var removedItem = removedItems[0];

                array.splice(toIndex, 0, removedItem);
            }
        }

        // inject code before method call
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
    };
})(htmlUi);

//# sourceURL=html-ui.extension.main.js