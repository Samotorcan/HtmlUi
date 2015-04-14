/// <reference path="references.ts" />

// run
module htmlUi {
    native.loadInternalScript('lodash.min.js');

    export var _: _.LoDashStatic = window['_'].noConflict();
}

// definitions
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
        newItems: Object[];
        newStartingIndex: number;
        oldStartingIndex: number;
    }

    export enum ObservableCollectionChangeAction {
        Add = 1,
        Remove = 2,
        Replace = 3,
        Move = 4
    }

    export interface INativeResponse<TValue> {
        type: NativeResponseType;
        exception: IJavascriptException;
        value: TValue;
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

// utility
module htmlUi.utility {
    export function argumentsToArray(args: IArguments): any[] {
        var argsArray = [];

        _.forEach(args, function (arg) {
            argsArray.push(arg);
        });

        return argsArray;
    }

    export function inject(func: Function, inject: Function): Function {
        return () => {
            inject.apply(this, arguments);
            return func.apply(this, arguments);
        };
    }

    export function shallowCopyCollection<T>(collection: T[]): T[] {
        if (collection == null)
            return null;

        var newCollection: T[] = [];
        _.forEach(collection, function (value, index) {
            newCollection[index] = value;
        });

        return newCollection;
    }

    export function isArrayShallowEqual<T>(firstArray: T[], secondArray: T[]): boolean {
        if (firstArray === secondArray)
            return true;

        if (firstArray == null || secondArray == null)
            return false;

        if (firstArray.length != secondArray.length)
            return false;

        return !_.any(firstArray, function (value, index) {
            return value !== secondArray[index];
        });
    }
}

// settings
module htmlUi.settings {
    // constants
    declare var _nativeRequestUrl: string;

    var _settings = (() => {
        // !inject-constants

        return {
            nativeRequestUrl: _nativeRequestUrl
        }
    })();

    export var nativeRequestUrl = _settings.nativeRequestUrl;
}

// main
module htmlUi {
    export function domReady(func: () => void): void {
        if (document.readyState === 'complete')
            setTimeout(func, 0);
        else
            document.addEventListener("DOMContentLoaded", func);
    }

    export function includeScript(scriptName: string, onload?: (ev: Event) => any): void {
        var scriptElement = document.createElement('script');
        document.body.appendChild(scriptElement);

        if (onload != null)
            scriptElement.onload = onload;

        scriptElement.src = scriptName;
    }
}