var app = angular.module('ControllerConstructorTestApp', ['htmlUi']);

app.factory('ControllerConstructorTest', ['htmlUi.controller', function (htmlUiController) {
    return htmlUiController.createController('ControllerConstructorTest');
}]);