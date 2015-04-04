module htmlUi {
    export interface IControllerChange {
        id: number;
        properties: { [name: string]: Object };
        observableCollections: { [name: string]: IObservableCollectionChanges };
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