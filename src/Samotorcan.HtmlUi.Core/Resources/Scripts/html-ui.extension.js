var htmlUi = htmlUi || {};

(function (htmlUi) {
    htmlUi.app = null;

    var native = {
        digest: function (controllers) {
            //!native function digest();
            digest(controllers);
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
    };

    function inject(func, inject) {
        return function () {
            inject.apply(this, arguments);
            return func.apply(this, arguments);
        };
    }
})(htmlUi);