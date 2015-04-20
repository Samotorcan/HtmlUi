var app = angular.module('todoListApp', ['ui.bootstrap', 'htmlUi']);

app.controller('todoListController', ['$scope', 'htmlUi.controller', 'focus', function ($scope, htmlUiController, focus) {
    htmlUiController.createObservableController('TodoListController', $scope);

    $scope.todo = '';
    $scope.buttonText = 'Add';

    var editIndex = null;

    $scope.addEditTodo = function () {
        if (editIndex == null) {
            if ($scope.todo != '')
                $scope.todos.push({ text: $scope.todo });
        } else {
            if ($scope.todo != '')
                $scope.todos[editIndex] = { text: $scope.todo };
            else
                $scope.removeTodo(editIndex);

            editIndex = null;
        }

        $scope.buttonText = 'Add';
        $scope.todo = '';
    };

    $scope.removeTodo = function (index) {
        $scope.todos.splice(index, 1);
    };

    $scope.editTodo = function (index) {
        $scope.todo = $scope.todos[index].text;
        editIndex = index;

        focus('input');

        $scope.buttonText = 'Edit';
    };
}]);

app.directive('focusOn', function () {
    return function (scope, elem, attr) {
        scope.$on('focusOn', function (e, name) {
            if (name === attr.focusOn) {
                elem[0].focus();
            }
        });
    };
});

app.factory('focus', function ($rootScope, $timeout) {
    return function (name) {
        $timeout(function () {
            $rootScope.$broadcast('focusOn', name);
        });
    }
});