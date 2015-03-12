var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    // !inject-constants
    nativeRequestUrl = nativeRequestUrl || null;

    htmlUi.init = function () {
        // services
        var injector = angular.injector(['ng']);
        var $q = injector.get('$q');

        // native functions TODO: make it private when done testing
        var native = htmlUi.native = (function () {
            var native = function (name, data) {
                var deferred = $q.defer();

                // !native function native();
                native(name, JSON.stringify(data), function (json) {
                    var response = JSON.parse(json);

                    if (response.type == 'Value')
                        deferred.resolve(response.value);
                    else if (response.type == 'Exception')
                        deferred.reject(response.exception);
                    else
                        deferred.resolve();
                });

                return deferred.promise;
            };

            return {
                syncControllerChanges: function (controllers) {
                    return nativeSynchronous('syncControllerChanges', controllers);
                },
                syncControllerChangesAsync: function (controllers) {
                    return native('syncControllerChanges', controllers);
                },

                getControllerNames: function () {
                    return nativeSynchronous('getControllerNames');
                },
                getControllerNamesAsync: function () {
                    return native('getControllerNames', null);
                },

                createController: function (name, id) {
                    return nativeSynchronous('createController', { name: name, id: id });
                },
                createControllerAsync: function (name, id) {
                    return native('createController', { name: name, id: id });
                },

                destroyController: function (id) {
                    return nativeSynchronous('destroyController', id);
                },
                destroyControllerAsync: function (id) {
                    return native('destroyController', id);
                },

                callMethod: function (id, name, args) {
                    return nativeSynchronous('callMethod', { id: id, name: name, args: args });
                },
                callMethodAsync: function (id, name, args) {
                    return native('callMethod', { id: id, name: name, args: args });
                },

                registerFunction: function (name, func) {
                    // !native function registerFunction();
                    registerFunction(name, func);
                },

                log: function (type, messageType, message) {
                    return nativeSynchronous('log', { type: type, messageType: messageType, message: message });
                }
            }
        })();

        // register functions
        native.registerFunction('syncControllerChanges', syncControllerChanges);

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
                angular.forEach(changes, function (properties, controllerId) {
                    if (properties != null && Object.keys(properties).length > 0)
                        controllers.push({ id: controllerId, properties: properties });
                });

                // call sync controller changes
                if (controllers.length > 0)
                    native.syncControllerChanges(controllers);

                // clear changes
                $rootScope.htmlUiChanges = {};
            }, true);
        }]);

        // create controllers
        var controllerNames = native.getControllerNames();
        var controllers = {};

        for (var i = 0; i < controllerNames.length; i++) {
            var controllerName = controllerNames[i];

            htmlUi.app.controller(controllerName, ['$scope', '$rootScope', function ($scope, $rootScope) {
                // save controller
                controllers[$scope.$id] = $scope;

                // create controller
                var controller = native.createController(controllerName, $scope.$id);

                // properties
                angular.forEach(controller.properties, function (property) {
                    var propertyName = property.name;
                    $scope[propertyName] = property.value;

                    // watch property
                    $scope.$watch(propertyName, function (newValue, oldValue) {
                        if (newValue !== oldValue) {
                            var changeProperties = $rootScope.htmlUiChanges[$scope.$id] = ($rootScope.htmlUiChanges[$scope.$id] || {});

                            changeProperties[propertyName] = newValue;
                        }
                    });
                });

                // methods
                angular.forEach(controller.methods, function (method) {
                    $scope[method.name] = function () {
                        return native.callMethod($scope.$id, method.name, argumentsToArray(arguments));
                    };
                });

                // destroy controller
                $scope.$on('$destroy', function () {
                    native.destroyController($scope.$id);
                });
            }]);
        }

        function syncControllerChanges(json) {
            var controllerChanges = JSON.parse(json);

            angular.forEach(controllerChanges, function (controllerChange) {
                var controller = controllers[controllerChange.Id];
                
                controller.$apply(function () {
                    angular.forEach(controllerChange.Properties, function (value, propertyName) {
                        controller[camelize(propertyName)] = value;
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

    // native synchronous call
    function nativeSynchronous(action, data) {
        var xhr = new XMLHttpRequest();
        var url = nativeRequestUrl + action;

        if (data != null) {
            xhr.open('POST', url, false);
            xhr.send(JSON.stringify(data));
        } else {
            xhr.open('GET', url, false);
            xhr.send();
        }

        var response = JSON.parse(xhr.responseText);

        if (response.type == 'Value')
            return response.value;

        if (response.type == 'Exception')
            throw response.exception;
    }

    function argumentsToArray(args) {
        var argsArray = [];

        for (var i = 0; i < args.length; i++)
            argsArray.push(args[i]);

        return argsArray;
    }

    function camelize(str) {
        return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
            if (+match === 0) return ""; // or if (/\s+/.test(match)) for white spaces
            return index == 0 ? match.toLowerCase() : match.toUpperCase();
        });
    }
})(htmlUi);