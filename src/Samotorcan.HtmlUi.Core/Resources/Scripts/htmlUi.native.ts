/// <reference path="references.ts" />

module htmlUi.native {
    export function syncControllerChanges(controllerChanges: IControllerChange[]): void {
        callNativeSync('syncControllerChanges', controllerChanges);
    }

    export function syncControllerChangesAsync(controllerChanges: IControllerChange[], callback?: (nativeResponse: NativeResponse<Object>) => void): void {
        callNativeAsync('syncControllerChanges', controllerChanges, callback);
    }

    export function getControllerNames(): string[] {
        return callNativeSync<string[]>('getControllerNames');
    }

    export function getControllerNamesAsync(callback?: (nativeResponse: NativeResponse<string[]>) => void): void {
        callNativeAsync('getControllerNames', null, callback);
    }

    export function createController(name: string): IControllerDescription {
        return callNativeSync<IControllerDescription>('createController', { name: name });
    }

    export function createControllerAsync(name: string, callback?: (nativeResponse: NativeResponse<IControllerDescription>) => void): void {
        callNativeAsync('createController', { name: name }, callback);
    }

    export function createObservableController(name: string): IObservableControllerDescription {
        return callNativeSync<IObservableControllerDescription>('createObservableController', { name: name });
    }

    export function createObservableControllerAsync(name: string, callback?: (nativeResponse: NativeResponse<IObservableControllerDescription>) => void): void {
        callNativeAsync('createObservableController', { name: name }, callback);
    }

    export function destroyController(id: number): void {
        callNativeSync('destroyController', id);
    }

    export function destroyControllerAsync(id: number, callback?: (nativeResponse: NativeResponse<Object>) => void): void {
        callNativeAsync('destroyController', id, callback);
    }

    export function callMethod<TValue>(id: number, name: string, args: Object[]): TValue {
        return callNativeSync<TValue>('callMethod', { id: id, name: name, args: args });
    }

    export function callMethodAsync<TValue>(id: number, name: string, args: Object[], callback?: (nativeResponse: NativeResponse<TValue>) => void): void {
        callNativeAsync('callMethod', { id: id, name: name, args: args }, callback);
    }

    export function callInternalMethod<TValue>(id: number, name: string, args: Object[]): TValue {
        return callNativeSync<TValue>('callMethod', { id: id, name: name, args: args, internalMethod: true });
    }

    export function callInternalMethodAsync<TValue>(id: number, name: string, args: Object[], callback?: (nativeResponse: NativeResponse<TValue>) => void): void {
        callNativeAsync('callMethod', { id: id, name: name, args: args, internalMethod: true }, callback);
    }

    export function registerFunction(name: string, func: Function): void {
        // !native function registerFunction();
        registerFunction(name, func);
    }

    export function log(type: string, messageType: string, message: string): void {
        callNativeSync('log', { type: type, messageType: messageType, message: message });
    }

    export function loadInternalScript(scriptName: string): void {
        // !native function loadInternalScript();
        loadInternalScript(scriptName);
    }

    export function callNativeAsync<TValue>(name: string, data: Object, callback?: (nativeResponse: NativeResponse<TValue>) => void): void {
        var jsonData = JSON.stringify(data);

        var internalCallback = (jsonResponse: string): void => {
            if (callback != null)
                callback(new NativeResponse<TValue>(jsonResponse));
        };

        native(name, jsonData, internalCallback);
    }

    function native(name: string, jsonData: string, callback?: (jsonResponse: string) => void): Object {
        // !native function native();
        return native(name, jsonData, callback);
    }

    export function callNativeSync<TValue>(action: string, data?: Object): TValue {
        var xhr = new XMLHttpRequest();
        var url = htmlUi.settings.nativeRequestUrl + action;

        if (data != null) {
            xhr.open('POST', url, false);
            xhr.send(JSON.stringify(data));
        } else {
            xhr.open('GET', url, false);
            xhr.send();
        }

        var response = <INativeResponse<TValue>>JSON.parse(xhr.responseText);

        if (response.type == NativeResponseType.Value)
            return response.value;

        if (response.type == NativeResponseType.Exception)
            throw response.exception;
    }

    export class NativeResponse<TValue> implements INativeResponse<TValue> {
        type: NativeResponseType;
        exception: IJavascriptException;
        value: TValue;

        getValue(): TValue {
            if (this.type == NativeResponseType.Exception)
                throw this.exception;

            if (this.type == NativeResponseType.Value)
                return this.value;

            return undefined;
        }

        constructor(jsonResponse: string) {
            var nativeResponse = <INativeResponse<TValue>>JSON.parse(jsonResponse);

            this.type = nativeResponse.type;
            this.exception = nativeResponse.exception;
            this.value = nativeResponse.value;
        }
    }
}