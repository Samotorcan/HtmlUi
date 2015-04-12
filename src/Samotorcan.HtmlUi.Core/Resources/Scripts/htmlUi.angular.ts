/// <reference path="angular.d.ts" />
/// <reference path="lodash.d.ts" />
/// <reference path="htmlUi.main.ts" />
/// <reference path="htmlUi.native.ts" />

// definitions
module htmlUi.angular {
    export class ControllerChange implements htmlUi.IControllerChange {
        id: number;
        properties: { [name: string]: Object };
        observableCollections: { [name: string]: ObservableCollectionChanges };

        get hasChanges(): boolean {
            return _.keys(this.properties).length != 0 ||
                (_.keys(this.observableCollections).length != 0 && _.any(this.observableCollections,(changes) => { return changes.hasChanges; }));
        }

        constructor(controllerId: number) {
            this.id = controllerId;
            this.properties = {};
            this.observableCollections = {};
        }

        getObservableCollection(propertyName: string): ObservableCollectionChanges {
            var observableCollections = _.find(this.observableCollections,(observableCollection) => { observableCollection.name == propertyName; });

            if (observableCollections == null) {
                observableCollections = new ObservableCollectionChanges();
                this.observableCollections[propertyName] = observableCollections;
            }

            return observableCollections;
        }

        setProperty(propertyName: string, value: any): void {
            this.properties[propertyName] = value;
        }

        getProperty(propertyName: string): any {
            return this.properties[propertyName];
        }

        hasProperty(propertyName: string): boolean {
            return _.has(this.properties, propertyName);
        }

        removeProperty(propertyName: string): void {
            if (this.hasProperty(propertyName))
                delete this.properties[propertyName];
        }

        addObservableCollectionChange(propertyName: string,
            action: ObservableCollectionChangeAction, newItem: Object, newStartingIndex: number, oldStartingIndex: number): void {

            this.getObservableCollection(propertyName).actions
                .push(new ObservableCollectionChange(action, newItem, newStartingIndex, oldStartingIndex));
        }

        hasObservableCollection(propertyName: string): boolean {
            return _.has(this.observableCollections, propertyName);
        }

        removeObservableCollection(propertyName: string): void {
            if (this.hasObservableCollection(propertyName))
                delete this.observableCollections[propertyName];
        }

        clear(): void {
            this.properties = {};
            this.observableCollections = {};
        }
    }

    export class ObservableCollectionChanges implements htmlUi.IObservableCollectionChanges {
        name: string;
        actions: ObservableCollectionChange[];

        constructor() {
            this.actions = [];
        }

        get hasChanges(): boolean {
            return _.keys(this.actions).length != 0;
        }
    }

    export class ObservableCollectionChange implements htmlUi.IObservableCollectionChange {
        action: ObservableCollectionChangeAction;
        newItems: Object[];
        newStartingIndex: number;
        oldStartingIndex: number;

        constructor(action: ObservableCollectionChangeAction, newItem: Object, newStartingIndex: number, oldStartingIndex: number) {
            this.action = action;
            this.newItems = [newItem];
            this.newStartingIndex = newStartingIndex;
            this.oldStartingIndex = oldStartingIndex;

            if (this.newItems == null)
                this.newItems = [];
        }
    }

    export class ControllerDataContainer {
        data: { [controllerId: number]: ControllerData };
        controllerChanges: ControllerChange[];

        get hasControllerChanges(): boolean {
            return _.any(this.controllerChanges,(controllerChange) => { return controllerChange.hasChanges; });
        }

        constructor() {
            this.data = {};
            this.controllerChanges = [];
        }

        addControllerData(controllerId: number): ControllerData {
            var controllerData: ControllerData = null;

            this.data[controllerId] = controllerData = new ControllerData(controllerId);
            this.controllerChanges.push(controllerData.change);

            return controllerData;
        }

        getControllerData(controllerId: number): ControllerData {
            return this.data[controllerId];
        }

        getControllerDataByScopeId(scopeId: number): ControllerData {
            return _.find(<_.Dictionary<ControllerData>>this.data,(controllerData) => {
                return controllerData.scopeId == scopeId;
            });
        }

        clearControllerChanges(): void {
            _.forEach(this.controllerChanges,(controllerChange) => {
                controllerChange.clear();
            });
        }
    }

    export class ControllerData {
        scopeId: number;
        controllerId: number;
        name: string;
        $scope: ng.IScope;
        propertyValues: { [name: string]: Object };
        observableCollectionValues: { [name: string]: Object[] };
        watches: { [name: string]: Function };
        change: ControllerChange;

        constructor(controllerId: number) {
            this.controllerId = controllerId;
            this.propertyValues = {};
            this.observableCollectionValues = {};
            this.watches = {};
            this.change = new ControllerChange(controllerId);
        }

        hasProperty(propertyName: string): boolean {
            return _.has(this.propertyValues, propertyName)
        }

        hasPropertyValue(propertyName: string, propertyValue: any): boolean {
            return this.hasProperty(propertyName) && this.propertyValues[propertyName] === propertyValue;
        }

        setPropertyValue(propertyName: string, propertyValue: any): void {
            this.propertyValues[propertyName] = propertyValue;
        }

        removePropertyValue(propertyName: string): void {
            if (this.hasProperty(propertyName))
                delete this.propertyValues[propertyName];
        }

        hasObservableCollection(propertyName: string): boolean {
            return _.has(this.observableCollectionValues, propertyName)
        }

        hasObservableCollectionValue(propertyName: string, observableCollectionValue: Object[]): boolean {
            return this.hasObservableCollection(propertyName) && utility.isArrayShallowEqual(this.observableCollectionValues[propertyName], observableCollectionValue);
        }

        setObservableCollectionValue(propertyName: string, observableCollectionValue: Object[]): void {
            this.observableCollectionValues[propertyName] = observableCollectionValue;
        }

        removeObservableCollectionValue(propertyName: string): void {
            if (this.hasObservableCollection(propertyName))
                delete this.observableCollectionValues[propertyName];
        }

        setControllerPropertyValue(propertyName: string, propertyValue: any): void {
            this.$scope[propertyName] = propertyValue;
        }

        hasWatch(propertyName: string): boolean {
            return _.has(this.watches, propertyName);
        }

        removeWatch(propertyName: string): void {
            if (this.hasWatch(propertyName)) {
                this.watches[propertyName]();

                delete this.watches[propertyName];
            }
        }

        addWatch(propertyName: string, watch: Function): void {
            this.removeWatch(propertyName);
            this.watches[propertyName] = watch;
        }
    }
}

// services
module htmlUi.angular {
    var _angular: ng.IAngularStatic = window['angular'];

    var _$q: ng.IQService = null;

    export var services = {
        get $q(): ng.IQService {
            if (_$q == null)
                _$q = _angular.injector(['ng']).get('$q');

            return _$q;
        }
    };
}

// native
module htmlUi.angular.native {
    export function syncControllerChanges(controllerChanges: ControllerChange[]): void {
        htmlUi.native.callNativeSync('syncControllerChanges', controllerChanges);
    }

    export function syncControllerChangesAsync(controllerChanges: ControllerChange[]): ng.IPromise<Object> {
        return callNativeAsync<Object>('syncControllerChanges', controllerChanges);
    }

    export function getControllerNames(): string[] {
        return htmlUi.native.callNativeSync<string[]>('getControllerNames');
    }

    export function getControllerNamesAsync(): ng.IPromise<string[]> {
        return callNativeAsync<string[]>('getControllerNames', null);
    }

    export function createController(name: string): IControllerDescription {
        return htmlUi.native.callNativeSync<IControllerDescription>('createController', { name: name });
    }

    export function createControllerAsync(name: string): ng.IPromise<IControllerDescription> {
        return callNativeAsync<IControllerDescription>('createController', { name: name });
    }

    export function createObservableController(name: string): IObservableControllerDescription {
        return htmlUi.native.callNativeSync<IObservableControllerDescription>('createObservableController', { name: name });
    }

    export function createObservableControllerAsync(name: string): ng.IPromise<IObservableControllerDescription> {
        return callNativeAsync<IObservableControllerDescription>('createObservableController', { name: name });
    }

    export function destroyController(id: number): void {
        htmlUi.native.callNativeSync('destroyController', id);
    }

    export function destroyControllerAsync(id: number): ng.IPromise<Object> {
        return callNativeAsync<Object>('destroyController', id);
    }

    export function callMethod<TValue>(id: number, name: string, args: Object[]): TValue {
        return htmlUi.native.callNativeSync<TValue>('callMethod', { id: id, name: name, args: args });
    }

    export function callMethodAsync<TValue>(id: number, name: string, args: Object[]): ng.IPromise<TValue> {
        return callNativeAsync<TValue>('callMethod', { id: id, name: name, args: args });
    }

    export function callInternalMethod<TValue>(id: number, name: string, args: Object[]): TValue {
        return htmlUi.native.callNativeSync<TValue>('callMethod', { id: id, name: name, args: args, internalMethod: true });
    }

    export function callInternalMethodAsync<TValue>(id: number, name: string, args: Object[]): ng.IPromise<TValue> {
        return callNativeAsync('callMethod', { id: id, name: name, args: args, internalMethod: true });
    }

    export function registerFunction(name: string, func: Function): void {
        htmlUi.native.registerFunction(name, func);
    }

    export function log(type: string, messageType: string, message: string): void {
        htmlUi.native.callNativeSync('log', { type: type, messageType: messageType, message: message });
    }

    function callNativeAsync<TValue>(name: string, data?: Object): ng.IPromise<TValue> {
        var deferred = services.$q.defer<Object>();

        htmlUi.native.callNativeAsync<TValue>(name, data, function (response) {
            if (response.type == NativeResponseType.Value)
                deferred.resolve(response.value);
            else if (response.type == NativeResponseType.Exception)
                deferred.reject(response.exception);
            else
                deferred.resolve();
        });

        return deferred.promise;
    }
}

// main
module htmlUi.angular {
    var _angular: ng.IAngularStatic = window['angular'];

    var _controllerDataContainer = new ControllerDataContainer();

    init();

    function init(): void {
        // register functions
        native.registerFunction('syncControllerChanges', syncControllerChanges);

        // module
        var htmlUiModule = _angular.module('htmlUi', []);

        // run
        htmlUiModule.run(['$rootScope', ($rootScope: ng.IRootScopeService) => {
            $rootScope['htmlUiControllerChanges'] = _controllerDataContainer.controllerChanges;

            addHtmlUiControllerChangesWatch($rootScope);
        }]);

        // controller
        htmlUiModule.factory('htmlUi.controller', [() => {
            var createObservableController = (controllerName: string, $scope: ng.IScope): ng.IScope => {
                var scopeId = $scope.$id;

                // create controller
                var observableController = native.createObservableController(controllerName);

                var controllerData = _controllerDataContainer.addControllerData(observableController.id);
                controllerData.name = controllerName;
                controllerData.$scope = $scope;
                controllerData.scopeId = $scope.$id;

                // properties
                _.forEach(observableController.properties,(property) => {
                    var propertyName = property.name;
                    $scope[propertyName] = property.value;

                    // watch observable collection
                    if (_.isArray(property.value))
                        addCollectionWatch(propertyName, $scope);

                    // watch property
                    addPropertyWatch(propertyName, $scope);
                });

                // methods
                _.forEach(observableController.methods,(method) => {
                    $scope[method.name] = () => {
                        return native.callMethod<Object>(observableController.id, method.name, utility.argumentsToArray(arguments));
                    };
                });

                // destroy controller
                $scope.$on('$destroy',() => {
                    native.destroyController(observableController.id);
                });

                // warm up native calls
                native.callInternalMethodAsync<Object>($scope.$id, 'warmUp', ['warmUp']).then(() => { });

                return $scope;
            };

            return {
                createObservableController: createObservableController
            };
        }]);
    }

    function addHtmlUiControllerChangesWatch($rootScope: ng.IRootScopeService): void {
        $rootScope.$watch('htmlUiControllerChanges',() => {
            if (!_controllerDataContainer.hasControllerChanges)
                return;

            try {
                native.syncControllerChanges(_controllerDataContainer.controllerChanges);
            } finally {
                _controllerDataContainer.clearControllerChanges();
            }
        }, true);
    }

    function addPropertyWatch(propertyName: string, $scope: ng.IScope): void {
        var scopeId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerDataByScopeId(scopeId);

        $scope.$watch(propertyName,(newValue, oldValue) => {
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

    function addCollectionWatch(propertyName: string, $scope: ng.IScope): void {
        var scopeId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerDataByScopeId(scopeId);

        controllerData.addWatch(propertyName, $scope.$watchCollection(propertyName,(newCollection: any[], oldCollection: any[]) => {
            if (newCollection !== oldCollection && !utility.isArrayShallowEqual(newCollection, oldCollection) &&
                !controllerData.hasObservableCollectionValue(propertyName, newCollection) &&
                !controllerData.change.hasProperty(propertyName)) {

                var compareValues = _.zip(oldCollection, newCollection);

                _.forEach(compareValues,(compareValue, index) => {
                    var oldValue = compareValue[0];
                    var newValue = compareValue[1];

                    if (index < oldCollection.length && index < newCollection.length) {
                        // replace
                        if (oldValue !== newValue) {
                            controllerData.change.addObservableCollectionChange(propertyName,
                                ObservableCollectionChangeAction.Replace, newValue, index, null);
                        }
                    } else if (index < oldCollection.length && index >= newCollection.length) {
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

    function removeCollectionWatch(propertyName: string, $scope: ng.IScope): void {
        var scopeId = $scope.$id;
        var controllerData = _controllerDataContainer.getControllerDataByScopeId(scopeId);

        controllerData.removeWatch(propertyName);
    }

    function syncControllerChanges(json: string): void {
        var controllerChanges = <ControllerChange[]>JSON.parse(json);

        _.forEach(controllerChanges,(controllerChange) => {
            var controllerId = controllerChange.id;
            var controllerData = _controllerDataContainer.getControllerData(controllerId);
            var controller = controllerData.$scope;

            controller.$apply(() => {
                // properties
                _.forEach(controllerChange.properties,(value, propertyName) => {
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

    function observableCollectionAddAction(collection: any[], change: ObservableCollectionChange): void {
        var insertIndex = change.newStartingIndex;
        var insertItems = change.newItems;

        _.forEach(insertItems,(insertItem) => {
            collection.splice(insertIndex, 0, insertItem);
            insertIndex++;
        });
    }

    function observableCollectionRemoveAction(collection: any[], change: ObservableCollectionChange): void {
        var removeIndex = change.oldStartingIndex;

        collection.splice(removeIndex, 1);
    }

    function observableCollectionReplaceAction(collection: any[], change: ObservableCollectionChange): void {
        var replaceIndex = change.newStartingIndex;
        var replaceItems = change.newItems;

        _.forEach(replaceItems,(replaceItem) => {
            collection[replaceIndex] = replaceItem;
            replaceIndex++;
        });
    }

    function observableCollectionMoveAction(collection: any[], change: ObservableCollectionChange): void {
        var fromIndex = change.oldStartingIndex;
        var toIndex = change.newStartingIndex;

        if (fromIndex == toIndex)
            return;

        var removedItems = collection.splice(fromIndex, 1);

        if (removedItems.length == 1) {
            var removedItem = removedItems[0];

            collection.splice(toIndex, 0, removedItem);
        }
    }
}