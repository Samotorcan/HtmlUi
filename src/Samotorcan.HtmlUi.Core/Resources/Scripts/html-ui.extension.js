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
                digest: function (controllers) {
                    return nativeSynchronous('digest', controllers);
                },
                digestAsync: function (controllers) {
                    return native('digest', controllers);
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

                log: function (type, messageType, message) {
                    return nativeSynchronous('log', { type: type, messageType: messageType, message: message });
                }
            }
        })();

        // create module
        htmlUi.app = angular.module('htmlUiApp', []);

        // inject controller creation
        htmlUi.app.controller = inject(htmlUi.app.controller, function (name) {
            
        });

        // digest
        htmlUi.app.run(['$rootScope', function ($rootScope) {
            // scope change
            $rootScope.$watch(function (rootScope) {
                var controllers = [];

                // prepare controller values
                for (var scope = rootScope.$$childHead; scope; scope = scope.$$nextSibling) {
                    var controller = { id: scope.$id, properties: {} };

                    angular.forEach(scope, function (value, key) {
                        if (key.charAt(0) != "$")
                            controller.properties[key] = value;
                    });

                    controllers.push(controller);
                }

                native.digest(controllers);
            });
        }]);

        // create controllers
        var controllerNames = native.getControllerNames();

        for (var i = 0; i < controllerNames.length; i++) {
            var controllerName = controllerNames[i];

            htmlUi.app.controller(controllerName, ['$scope', function ($scope) {
                // create controller
                var controller = native.createController(controllerName, $scope.$id);

                // properties
                for (var j = 0; j < controller.properties.length; j++) {
                    var property = controller.properties[j];

                    $scope[property.name] = property.value;
                }

                // methods
                for (var j = 0; j < controller.methods.length; j++) {
                    var method = controller.methods[j];

                    $scope[method.name] = function () {
                        return native.callMethod($scope.$id, method.name, argumentsToArray(arguments));
                    };
                }

                // destroy controller
                $scope.$on('$destroy', function () {
                    native.destroyController($scope.$id);
                })
            }]);
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
})(htmlUi);