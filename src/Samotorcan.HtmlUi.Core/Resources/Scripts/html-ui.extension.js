var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    // !inject-constants
    nativeRequestUrl = nativeRequestUrl || null;

    // TODO: make it private when done testing
    var native = htmlUi.native = (function () {
        var native = function (name, data, callback) {
            // !native function native();
            native(name, JSON.stringify(data), convertCallback(callback));
        };

        return {
            digest: function (controllers) {
                return nativeSynchronous('digest', controllers);
            },
            digestAsync: function (controllers, callback) {
                native('digest', controllers, callback);
            },

            getControllerNames: function () {
                return nativeSynchronous('controller-names');
            },
            getControllerNamesAsync: function (callback) {
                native('getControllerNames', null, callback);
            },

            createController: function (name, id) {
                return nativeSynchronous('create-controller', { name: name, id: id });
            },
            createControllerAsync: function (name, id, callback) {
                native('createController', { name: name, id: id }, callback);
            },

            destroyController: function (id) {
                return nativeSynchronous('destroy-controller', id);
            },
            destroyControllerAsync: function (id, callback) {
                native('destroyController', id, callback);
            },

            log: function (type, messageType, message) {
                return nativeSynchronous('log', { type: type, messageType: messageType, message: message });
            }
        }
    })();

    htmlUi.init = function () {
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

        // create controllers   // TODO: add methods
        var controllerNames = native.getControllerNames();

        for (var i = 0; i < controllerNames.length; i++) {
            var controllerName = controllerNames[i];

            htmlUi.app.controller(controllerName, ['$scope', function ($scope) {
                // create controller
                var controller = native.createController(controllerName, $scope.$id);
                console.log($scope.$id);

                // properties
                for (var j = 0; j < controller.properties.length; j++) {
                    var property = controller.properties[j];

                    $scope[property.name] = property.value;
                }

                // methods
                for (var j = 0; j < controller.methods.length; j++) {
                    var method = controller.methods[j];

                    $scope[method.name] = function () {
                        return native.callMethod($scope.$id, method.name, arguments);
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

        var response = xhr.responseText;

        return response != null && response != '' && xhr.status == 200 ? JSON.parse(response) : undefined;
    }

    function convertCallback(callback) {
        return function (json) {
            if (callback != null)
                callback(JSON.parse(json))
        }
    }
})(htmlUi);