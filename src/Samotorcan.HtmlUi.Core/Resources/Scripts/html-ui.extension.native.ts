/// <reference path="references.ts" />

module htmlUi.native {
    export function syncControllerChanges(controllerChanges: ControllerChange[]): void {
        nativeSynchronous('syncControllerChanges', controllerChanges);
    }

    export function syncControllerChangesAsync(controllerChanges: ControllerChange[]): angular.IPromise<Object> {
        return callNative('syncControllerChanges', controllerChanges);
    }

    export function getControllerNames(): string[] {
        return <string[]>nativeSynchronous('getControllerNames');
    }

    export function getControllerNamesAsync(): angular.IPromise<string[]> {
        return callNative('getControllerNames');
    }

    export function createController(name: string): IControllerDescription {
        return <IControllerDescription>nativeSynchronous('createController', { name: name });
    }

    export function createControllerAsync(name: string): angular.IPromise<IControllerDescription> {
        return callNative('createController', { name: name });
    }

    export function createObservableController(name: string): IObservableControllerDescription {
        return <IObservableControllerDescription>nativeSynchronous('createObservableController', { name: name });
    }

    export function createObservableControllerAsync(name: string): angular.IPromise<IObservableControllerDescription> {
        return callNative('createObservableController', { name: name });
    }

    export function destroyController(id: number): void {
        nativeSynchronous('destroyController', id);
    }

    export function destroyControllerAsync(id: number): angular.IPromise<Object> {
        return callNative('destroyController', id);
    }

    export function callMethod(id: number, name: string, args: Object[]): Object {
        return nativeSynchronous('callMethod', { id: id, name: name, args: args });
    }

    export function callMethodAsync(id: number, name: string, args: Object[]): angular.IPromise<Object> {
        return callNative('callMethod', { id: id, name: name, args: args });
    }

    export function callInternalMethod(id: number, name: string, args: Object[]): Object {
        return nativeSynchronous('callMethod', { id: id, name: name, args: args, internalMethod: true });
    }

    export function callInternalMethodAsync(id: number, name: string, args: Object[]): angular.IPromise<Object> {
        return callNative('callMethod', { id: id, name: name, args: args, internalMethod: true });
    }

    export function registerFunction(name: string, func: Function): void {
        // !native function registerFunction();
        registerFunction(name, func);
    }

    export function log(type: string, messageType: string, message: string): void {
        nativeSynchronous('log', { type: type, messageType: messageType, message: message });
    }

    function callNative(name: string, data?: Object): angular.IPromise<Object> {
        var deferred = services.$q.defer<Object>();

        native(name, JSON.stringify(data), function (json) {
            var response = <INativeResponse>JSON.parse(json);

            if (response.type == NativeResponseType.Value)
                deferred.resolve(response.value);
            else if (response.type == NativeResponseType.Exception)
                deferred.reject(response.exception);
            else
                deferred.resolve();
        });

        return deferred.promise;
    }

    function native(name: string, jsonData: string, callback: Function): Object {
        // !native function native();
        return native(name, jsonData, callback);
    }

    function nativeSynchronous(action: string, data?: Object): Object {
        var xhr = new XMLHttpRequest();
        var url = htmlUi.settings.nativeRequestUrl + action;

        if (data != null) {
            xhr.open('POST', url, false);
            xhr.send(JSON.stringify(data));
        } else {
            xhr.open('GET', url, false);
            xhr.send();
        }

        var response = <INativeResponse>JSON.parse(xhr.responseText);

        if (response.type == NativeResponseType.Value)
            return response.value;

        if (response.type == NativeResponseType.Exception)
            throw response.exception;
    }
}