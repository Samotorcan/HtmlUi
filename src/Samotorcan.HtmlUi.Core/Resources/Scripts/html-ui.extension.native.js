/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    var native;
    (function (_native) {
        function syncControllerChanges(controllerChanges) {
            nativeSynchronous('syncControllerChanges', controllerChanges);
        }
        _native.syncControllerChanges = syncControllerChanges;
        function syncControllerChangesAsync(controllerChanges) {
            return callNative('syncControllerChanges', controllerChanges);
        }
        _native.syncControllerChangesAsync = syncControllerChangesAsync;
        function getControllerNames() {
            return nativeSynchronous('getControllerNames');
        }
        _native.getControllerNames = getControllerNames;
        function getControllerNamesAsync() {
            return callNative('getControllerNames');
        }
        _native.getControllerNamesAsync = getControllerNamesAsync;
        function createController(name) {
            return nativeSynchronous('createController', { name: name });
        }
        _native.createController = createController;
        function createControllerAsync(name) {
            return callNative('createController', { name: name });
        }
        _native.createControllerAsync = createControllerAsync;
        function createObservableController(name) {
            return nativeSynchronous('createObservableController', { name: name });
        }
        _native.createObservableController = createObservableController;
        function createObservableControllerAsync(name) {
            return callNative('createObservableController', { name: name });
        }
        _native.createObservableControllerAsync = createObservableControllerAsync;
        function destroyController(id) {
            nativeSynchronous('destroyController', id);
        }
        _native.destroyController = destroyController;
        function destroyControllerAsync(id) {
            return callNative('destroyController', id);
        }
        _native.destroyControllerAsync = destroyControllerAsync;
        function callMethod(id, name, args) {
            return nativeSynchronous('callMethod', { id: id, name: name, args: args });
        }
        _native.callMethod = callMethod;
        function callMethodAsync(id, name, args) {
            return callNative('callMethod', { id: id, name: name, args: args });
        }
        _native.callMethodAsync = callMethodAsync;
        function callInternalMethod(id, name, args) {
            return nativeSynchronous('callMethod', { id: id, name: name, args: args, internalMethod: true });
        }
        _native.callInternalMethod = callInternalMethod;
        function callInternalMethodAsync(id, name, args) {
            return callNative('callMethod', { id: id, name: name, args: args, internalMethod: true });
        }
        _native.callInternalMethodAsync = callInternalMethodAsync;
        function registerFunction(name, func) {
            // !native function registerFunction();
            registerFunction(name, func);
        }
        _native.registerFunction = registerFunction;
        function log(type, messageType, message) {
            nativeSynchronous('log', { type: type, messageType: messageType, message: message });
        }
        _native.log = log;
        function callNative(name, data) {
            var deferred = htmlUi.services.$q.defer();
            native(name, JSON.stringify(data), function (json) {
                var response = JSON.parse(json);
                if (response.type == 1 /* Value */)
                    deferred.resolve(response.value);
                else if (response.type == 3 /* Exception */)
                    deferred.reject(response.exception);
                else
                    deferred.resolve();
            });
            return deferred.promise;
        }
        function native(name, jsonData, callback) {
            // !native function native();
            return native(name, jsonData, callback);
        }
        function nativeSynchronous(action, data) {
            var xhr = new XMLHttpRequest();
            var url = htmlUi.settings.nativeRequestUrl + action;
            if (data != null) {
                xhr.open('POST', url, false);
                xhr.send(JSON.stringify(data));
            }
            else {
                xhr.open('GET', url, false);
                xhr.send();
            }
            var response = JSON.parse(xhr.responseText);
            if (response.type == 1 /* Value */)
                return response.value;
            if (response.type == 3 /* Exception */)
                throw response.exception;
        }
    })(native = htmlUi.native || (htmlUi.native = {}));
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.native.js.map