var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    //!inject-constants
    htmlUi.nativeRequestUrl = htmlUi.nativeRequestUrl || null;

    var native = {
        digest: function (controllers) {
            console.log('digest call'); // TEMP: debug

            //!native function digest();
            digest(controllers);
        },
        createControllers: function () {
            console.log('create controllers call'); // TEMP: debug

            return nativeSynchronous('create-controllers');
        }
    };

    htmlUi.init = function () {
        // create module
        htmlUi.app = angular.module('htmlUiApp', []);

        // inject controller creation
        htmlUi.app.controller = inject(htmlUi.app.controller, function (name) {
            console.log('controller \'' + name + '\' created');
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
        console.log(controllers);

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
    function nativeSynchronous(action) {
        var xhr = new XMLHttpRequest();

        xhr.open('GET', htmlUi.nativeRequestUrl + action, false);
        xhr.send();

        return JSON.parse(xhr.responseText);
    }
})(htmlUi);