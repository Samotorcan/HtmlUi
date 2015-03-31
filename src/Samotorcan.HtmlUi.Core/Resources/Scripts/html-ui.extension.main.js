var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    // !inject-constants
    nativeRequestUrl = nativeRequestUrl || null;

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
                var controllers = [];

                // prepare changes
                _.forEach(changes, function (properties, controllerId) {
                    if (properties != null && _.keys(properties).length > 0)
                        controllers.push({ id: controllerId, properties: properties });
                });

                // call sync controller changes
                if (controllers.length > 0) {
                    try {
                        htmlUi._native.syncControllerChanges(controllers);
                    } finally {
                        $rootScope.htmlUiChanges = {};
                    }
                }
            }, true);
        }]);

        // create controllers
        var controllerNames = htmlUi._native.getControllerNames();
        var controllers = {};

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

                    // watch property
                    $scope.$watch(propertyName, function (newValue, oldValue) {
                        var values = controllerValues[$scope.$id] || {};
                        var hasValue = _.has(values, propertyName);

                        // ignore sync if the value is already set in the server controller
                        if (newValue !== oldValue && (!hasValue || values[propertyName] !== newValue)) {
                            var changeProperties = $rootScope.htmlUiChanges[$scope.$id] = ($rootScope.htmlUiChanges[$scope.$id] || {});

                            changeProperties[propertyName] = newValue;
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
                        // TODO: implement
                    });
                });
            });
        }
    };

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
})(htmlUi);

//# sourceURL=html-ui.extension.main.js