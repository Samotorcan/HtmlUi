var app = angular.module('myApp', ['htmlUi']);

app.controller('GreetingController', ['$scope', '$rootScope', 'htmlUi.controller', function ($scope, $rootScope, htmlUiController) {
    htmlUiController.createObservableController('GreetingController', $scope);

    $scope.greeting = 'Hola!';
}]);