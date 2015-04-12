/// <reference path="htmlUi.main.ts" />
var htmlUi;
(function (htmlUi) {
    var native;
    (function (_native) {
        function syncControllerChanges(controllerChanges) {
            callNativeSync('syncControllerChanges', controllerChanges);
        }
        _native.syncControllerChanges = syncControllerChanges;
        function syncControllerChangesAsync(controllerChanges, callback) {
            callNativeAsync('syncControllerChanges', controllerChanges, callback);
        }
        _native.syncControllerChangesAsync = syncControllerChangesAsync;
        function getControllerNames() {
            return callNativeSync('getControllerNames');
        }
        _native.getControllerNames = getControllerNames;
        function getControllerNamesAsync(callback) {
            callNativeAsync('getControllerNames', null, callback);
        }
        _native.getControllerNamesAsync = getControllerNamesAsync;
        function createController(name) {
            return callNativeSync('createController', { name: name });
        }
        _native.createController = createController;
        function createControllerAsync(name, callback) {
            callNativeAsync('createController', { name: name }, callback);
        }
        _native.createControllerAsync = createControllerAsync;
        function createObservableController(name) {
            return callNativeSync('createObservableController', { name: name });
        }
        _native.createObservableController = createObservableController;
        function createObservableControllerAsync(name, callback) {
            callNativeAsync('createObservableController', { name: name }, callback);
        }
        _native.createObservableControllerAsync = createObservableControllerAsync;
        function destroyController(id) {
            callNativeSync('destroyController', id);
        }
        _native.destroyController = destroyController;
        function destroyControllerAsync(id, callback) {
            callNativeAsync('destroyController', id, callback);
        }
        _native.destroyControllerAsync = destroyControllerAsync;
        function callMethod(id, name, args) {
            return callNativeSync('callMethod', { id: id, name: name, args: args });
        }
        _native.callMethod = callMethod;
        function callMethodAsync(id, name, args, callback) {
            callNativeAsync('callMethod', { id: id, name: name, args: args }, callback);
        }
        _native.callMethodAsync = callMethodAsync;
        function callInternalMethod(id, name, args) {
            return callNativeSync('callMethod', { id: id, name: name, args: args, internalMethod: true });
        }
        _native.callInternalMethod = callInternalMethod;
        function callInternalMethodAsync(id, name, args, callback) {
            callNativeAsync('callMethod', { id: id, name: name, args: args, internalMethod: true }, callback);
        }
        _native.callInternalMethodAsync = callInternalMethodAsync;
        function registerFunction(name, func) {
            // !native function registerFunction();
            registerFunction(name, func);
        }
        _native.registerFunction = registerFunction;
        function log(type, messageType, message) {
            callNativeSync('log', { type: type, messageType: messageType, message: message });
        }
        _native.log = log;
        function callNativeAsync(name, data, callback) {
            var jsonData = JSON.stringify(data);
            var internalCallback = function (jsonResponse) {
                if (callback != null)
                    callback(new NativeResponse(jsonResponse));
            };
            native(name, jsonData, internalCallback);
        }
        _native.callNativeAsync = callNativeAsync;
        function native(name, jsonData, callback) {
            // !native function native();
            return native(name, jsonData, callback);
        }
        function callNativeSync(action, data) {
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
        _native.callNativeSync = callNativeSync;
        var NativeResponse = (function () {
            function NativeResponse(jsonResponse) {
                var nativeResponse = JSON.parse(jsonResponse);
                this.type = nativeResponse.type;
                this.exception = nativeResponse.exception;
                this.value = nativeResponse.value;
            }
            NativeResponse.prototype.getValue = function () {
                if (this.type == 3 /* Exception */)
                    throw this.exception;
                if (this.type == 1 /* Value */)
                    return this.value;
                return undefined;
            };
            return NativeResponse;
        })();
        _native.NativeResponse = NativeResponse;
    })(native = htmlUi.native || (htmlUi.native = {}));
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=htmlUi.native.js.map