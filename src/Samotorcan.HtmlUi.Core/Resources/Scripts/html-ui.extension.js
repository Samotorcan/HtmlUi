var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    // !inject-constants
    nativeRequestUrl = nativeRequestUrl || null;

    // TODO: make it private when done testing
    var native = htmlUi.native = {
        digest: function (controllers) {
            return nativeSynchronous('digest', controllers);
        },

        digestAsync: function (controllers, callback) {
            // !native function digest();
            digest(JSON.stringify(controllers), callback);
        },

        createControllers: function () {
            return nativeSynchronous('create-controllers');
        }
    };

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
        var controllers = native.createControllers();

        for (var i = 0; i < controllers.length; i++) {
            var controller = controllers[i];

            htmlUi.app.controller(controller.name, ['$scope', function ($scope) {
                // properties
                for (var j = 0; j < controller.properties.length; j++) {
                    var property = controller.properties[j];

                    $scope[property.name] = property.value;
                }
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
})(htmlUi);