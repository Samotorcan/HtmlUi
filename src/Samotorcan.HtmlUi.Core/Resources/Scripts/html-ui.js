(function (htmlUi) {
    htmlUi.app = angular.module('htmlUiApp', []);

    htmlUi.app.run(['$rootScope', function ($rootScope) {
        $rootScope.$watch(function () {
            // print values
            for (var cs = arguments[0].$$childHead; cs; cs = cs.$$nextSibling) {
                angular.forEach(cs, function (value, key) {
                    if (key.charAt(0) != "$")
                        console.log(key + "=" + value);
                });
            }
        });
    }]);

    htmlUi.app.controller('GreetingController', ['$scope', function ($scope) {
        $scope.greeting = 'Hola!';
    }]);

})(window.htmlUi || {});