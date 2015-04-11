module htmlUi {
    export interface IControllerChange {
        id: number;
        properties: { [name: string]: Object };
        observableCollections: { [name: string]: ObservableCollectionChanges };
    }

    export class ControllerChange implements IControllerChange {
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

    export class ObservableCollectionChanges {
        name: string;
        actions: ObservableCollectionChange[];

        constructor() {
            this.actions = [];
        }

        get hasChanges(): boolean {
            return _.keys(this.actions).length != 0;
        }
    }

    export class ObservableCollectionChange {
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

    export enum ObservableCollectionChangeAction {
        Add = 1,
        Remove = 2,
        Replace = 3,
        Move = 4
    }

    export class ControllerDataContainer {
        data: { [controllerId: number]: ControllerData };
        controllerChanges: ControllerChange[];

        get hasControllerChanges(): boolean {
            return _.any(this.controllerChanges, (controllerChange) => { return controllerChange.hasChanges; });
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
            return _.find(<_.Dictionary<ControllerData>>this.data, (controllerData) => {
                return controllerData.scopeId == scopeId;
            });
        }

        clearControllerChanges(): void {
            _.forEach(this.controllerChanges, (controllerChange) => {
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

    export interface INativeResponse {
        type: NativeResponseType;
        exception: IJavascriptException;
        value: Object;
    }

    export enum NativeResponseType {
        Value = 1,
        Undefined = 2,
        Exception = 3
    }

    export interface IJavascriptException {
        type: string;
        message: string;
        additionalData: { [name: string]: Object };
        innerException: IJavascriptException;
    }

    export interface IControllerDescription {
        id: number;
        name: string;
        methods: IControllerMethodDescription[];
    }

    export interface IObservableControllerDescription extends IControllerDescription {
        properties: IControllerPropertyDescription[];
    }

    export interface IControllerPropertyDescription extends IControllerPropertyBase {
        value: Object;
    }

    export interface IControllerPropertyBase {
        name: string;
    }

    export interface IControllerMethodDescription extends IControllerMethodBase {

    }

    export interface IControllerMethodBase {
        name: string;
    }
} 