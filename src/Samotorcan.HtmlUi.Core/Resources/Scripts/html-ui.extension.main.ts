/// <reference path="references.ts" />

module htmlUi {
    var _controllerDataContainer = new ControllerDataContainer();

    export function init() {
        var controllerNames = htmlUi.native.getControllerNames();

        // register functions
        htmlUi.native.registerFunction('syncControllerChanges', syncControllerChanges);

        // create module
        var app = angular.module('htmlUiApp', []);

        app.run(['$rootScope', ($rootScope: ng.IRootScopeService) => {
            $rootScope['htmlUiControllerChanges'] = _controllerDataContainer.controllerChanges;

            addHtmlUiControllerChangesWatch($rootScope);
        }]);

        // create controllers
        _.forEach(controllerNames, (controllerName) => {
            app.controller(controllerName, ['$scope', ($scope: ng.IScope) => {
                var controllerId = $scope.$id;
                var controllerData = _controllerDataContainer.getControllerData(controllerId);

                controllerData.name = controllerName;
                controllerData.$scope = $scope;

                // create controller
                var controller = htmlUi.native.createController(controllerName, $scope.$id);

                // properties
                _.forEach(controller.properties, (property) => {
                    var propertyName = property.name;
                    $scope[propertyName] = property.value;

                    // watch observable collection
                    if (_.isArray(property.value))
                        addCollectionWatch(propertyName, $scope);

                    // watch property
                    addPropertyWatch(propertyName, $scope);
                });

                // methods
                _.forEach(controller.methods, (method) => {
                    $scope[method.name] = () => {
                        return htmlUi.native.callMethod($scope.$id, method.name, utility.argumentsToArray(arguments));
                    };
                });

                // destroy controller
                $scope.$on('$destroy', () => {
                    htmlUi.native.destroyController($scope.$id);
                });

                // warm up native calls
                htmlUi.native.callInternalMethodAsync($scope.$id, 'warmUp', ['warmUp']).then(() => { });
            }]);
        });
    }

    function addHtmlUiControllerChangesWatch($rootScope: ng.IRootScopeService): void {
        $rootScope.$watch('htmlUiControllerChanges', () => {
            if (!_controllerDataContainer.hasControllerChanges)
                return;

            try {
                htmlUi.native.syncControllerChanges(_controllerDataContainer.controllerChanges);
            } finally {
                _controllerDataContainer.clearControllerChanges();
            }
        }, true);
    }

    function addPropertyWatch(propertyName: string, $scope: ng.IScope) {
        var controllerId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerData(controllerId);

        $scope.$watch(propertyName, (newValue, oldValue) => {
            if (newValue !== oldValue && !controllerData.hasPropertyValue(propertyName, newValue)) {
                controllerData.change.setProperty(propertyName, newValue);

                if (_.isArray(oldValue))
                    removeCollectionWatch(propertyName, $scope);

                if (_.isArray(newValue))
                    addCollectionWatch(propertyName, $scope);

                controllerData.change.removeObservableCollection(propertyName);
            }

            controllerData.removePropertyValue(propertyName);
        });
    }

    function addCollectionWatch(propertyName: string, $scope: ng.IScope) {
        var controllerId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerData(controllerId);

        controllerData.addWatch(propertyName, $scope.$watchCollection(propertyName,(newArray, oldArray) => {
            if (newArray !== oldArray && !utility.isArrayShallowEqual(newArray, oldArray) &&
                !controllerData.hasObservableCollectionValue(propertyName, newArray) &&
                !controllerData.change.hasProperty(propertyName)) {

                var compareValues = _.zip(oldArray, newArray);

                _.forEach(compareValues,(compareValue, index) => {
                    var oldValue = compareValue[0];
                    var newValue = compareValue[1];

                    if (index < oldArray.length && index < newArray.length) {
                        // replace
                        if (oldValue !== newValue) {
                            controllerData.change.addObservableCollectionChange(propertyName,
                                ObservableCollectionChangeAction.Replace, newValue, index, null);
                        }
                    } else if (index < oldArray.length && index >= newArray.length) {
                        // remove
                        controllerData.change.addObservableCollectionChange(propertyName,
                            ObservableCollectionChangeAction.Remove, null, null, index);
                    } else {
                        // add
                        controllerData.change.addObservableCollectionChange(propertyName,
                            ObservableCollectionChangeAction.Add, newValue, index, null);
                    }
                });
            }

            controllerData.removeObservableCollectionValue(propertyName);
        }));
    }

    function removeCollectionWatch(propertyName, $scope) {
        var controllerId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerData(controllerId);

        controllerData.removeWatch(propertyName);
    }

    function syncControllerChanges(json) {
        var controllerChanges = <ControllerChange[]>JSON.parse(json);

        _.forEach(controllerChanges, (controllerChange) => {
            var controllerId = controllerChange.id;
            var controllerData = _controllerDataContainer.getControllerData(controllerId);
            var controller = controllerData.$scope;

            controller.$apply(() => {
                // properties
                _.forEach(controllerChange.properties, (value, propertyName) => {
                    var propertyName = _.camelCase(propertyName);

                    controllerData.setControllerPropertyValue(propertyName, value);
                    controllerData.setPropertyValue(propertyName, value);
                });

                // observable collections
                _.forEach(controllerChange.observableCollections,(changes, propertyName) => {
                    var propertyName = _.camelCase(propertyName);

                    if (!_.isArray(controller[propertyName]))
                        controller[propertyName] = [];

                    var collection: any[] = controller[propertyName];

                    _.forEach(changes.actions,(change) => {
                        switch (change.action) {
                            case ObservableCollectionChangeAction.Add:
                                observableCollectionAddAction(collection, change);
                                break;
                            case ObservableCollectionChangeAction.Remove:
                                observableCollectionRemoveAction(collection, change);
                                break;
                            case ObservableCollectionChangeAction.Replace:
                                observableCollectionReplaceAction(collection, change);
                                break;
                            case ObservableCollectionChangeAction.Move:
                                observableCollectionMoveAction(collection, change);
                                break;
                        }
                    });

                    controllerData.setObservableCollectionValue(propertyName, utility.shallowCopyCollection(collection));
                });
            });
        });
    }

    function observableCollectionAddAction(array: any[], change: ObservableCollectionChange) {
        var insertIndex = change.newStartingIndex;
        var insertItems = change.newItems;

        _.forEach(insertItems, (insertItem) => {
            array.splice(insertIndex, 0, insertItem);
            insertIndex++;
        });
    }

    function observableCollectionRemoveAction(array: any[], change: ObservableCollectionChange) {
        var removeIndex = change.oldStartingIndex;

        array.splice(removeIndex, 1);
    }

    function observableCollectionReplaceAction(array: any[], change: ObservableCollectionChange) {
        var replaceIndex = change.newStartingIndex;
        var replaceItems = change.newItems;

        _.forEach(replaceItems, (replaceItem) => {
            array[replaceIndex] = replaceItem;
            replaceIndex++;
        });
    }

    function observableCollectionMoveAction(array: any[], change: ObservableCollectionChange) {
        var fromIndex = change.oldStartingIndex;
        var toIndex = change.newStartingIndex;

        if (fromIndex == toIndex)
            return;

        var removedItems = array.splice(fromIndex, 1);

        if (removedItems.length == 1) {
            var removedItem = removedItems[0];

            array.splice(toIndex, 0, removedItem);
        }
    }
} 