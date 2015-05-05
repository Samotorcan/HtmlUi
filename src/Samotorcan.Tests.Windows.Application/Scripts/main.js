var app = angular.module('myApp', ['htmlUi']);

app.controller('GreetingController', ['$scope', 'htmlUi.controller', function ($scope, htmlUiController) {
    htmlUiController.createObservableController('GreetingController', $scope);

    $scope.greeting = 'Hola!';
}]);