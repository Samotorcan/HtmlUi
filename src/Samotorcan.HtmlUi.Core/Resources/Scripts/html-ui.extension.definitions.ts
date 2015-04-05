module htmlUi {
    export interface IControllerChanges {
        changes: IControllerChange[];
        hasChanges: boolean;
        getChange(id: number): IControllerChange;
        clear(): void;
    }

    export class ControllerChanges implements IControllerChanges {
        changes: IControllerChange[];

        get hasChanges(): boolean {
            return _.any(this.changes,(change) => { return change.hasChanges; });
        }

        constructor() {
            this.changes = [];
        }

        getChange(id: number): IControllerChange {
            var change = _.find(this.changes,(change) => { change.id == id; });

            if (change == null) {
                change = new ControllerChange(id);
                this.changes.push(change);
            }

            return change;
        }

        clear(): void {
            this.changes = [];
        }
    }

    export interface IControllerChange {
        id: number;
        properties: { [name: string]: Object };
        observableCollections: { [name: string]: IObservableCollectionChanges };
        hasChanges: boolean;
    }

    export class ControllerChange implements IControllerChange {
        id: number;
        properties: { [name: string]: Object };
        observableCollections: { [name: string]: IObservableCollectionChanges };

        get hasChanges(): boolean {
            return _.keys(this.properties).length != 0 ||
                (_.keys(this.observableCollections).length != 0 && _.all(this.observableCollections, (changes) => { return changes.actions.length > 0; }));
        }

        constructor(id: number) {
            this.id = id;
            this.properties = {};
            this.observableCollections = {};
        }
    }

    export interface IObservableCollectionChanges {
        name: string;
        actions: IObservableCollectionChange[];
    }

    export interface IObservableCollectionChange {
        action: ObservableCollectionChangeAction;
        newItems?: Object[];
        newStartingIndex?: number;
        oldStartingIndex?: number;
    }

    export enum ObservableCollectionChangeAction {
        Add = 1,
        Remove = 2,
        Replace = 3,
        Move = 4
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
        name: string;
        properties: IControllerPropertyDescription[];
        methods: IControllerMethodDescription[];
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