var app = angular.module('todoListApp', ['ui.bootstrap', 'htmlUi']);

app.controller('todoListController', ['$scope', 'focus', function ($scope, focus) {
    $scope.todos = [];
    $scope.todo = '';

    var editIndex = null;
    var editWatch = null;

    $scope.addEditTodo = function () {
        if (editIndex == null) {
            if ($scope.todo != '')
                $scope.todos.push({ text: $scope.todo });
        } else {
            if ($scope.todo != '')
                $scope.todos[editIndex].text = $scope.todo;
            else
                $scope.removeTodo(editIndex);

            editIndex = null;
            editWatch();
        }

        $scope.todo = '';
    };

    $scope.removeTodo = function (index) {
        $scope.todos.splice(index, 1);
    };

    $scope.editTodo = function (index) {
        $scope.todo = $scope.todos[index].text;
        editIndex = index;

        editWatch = $scope.$watch('todo', function () {
            $scope.todos[index].text = $scope.todo;
        });

        focus('input');
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