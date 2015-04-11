var app = angular.module('myApp', []);

app.controller('GreetingController', ['$scope', '$rootScope', 'htmlUi.controller', function ($scope, $rootScope, htmlUiController) {
    htmlUiController.createObservableController('GreetingController', $scope);

    $scope.greeting = 'Hola!';
}]);