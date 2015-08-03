/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    var native;
    (function (native_1) {
        function syncControllerChanges(controllerChanges) {
            callNativeSync('syncControllerChanges', controllerChanges);
        }
        native_1.syncControllerChanges = syncControllerChanges;
        function syncControllerChangesAsync(controllerChanges, callback) {
            callNativeAsync('syncControllerChanges', controllerChanges, callback);
        }
        native_1.syncControllerChangesAsync = syncControllerChangesAsync;
        function getControllerNames() {
            return callNativeSync('getControllerNames');
        }
        native_1.getControllerNames = getControllerNames;
        function getControllerNamesAsync(callback) {
            callNativeAsync('getControllerNames', null, callback);
        }
        native_1.getControllerNamesAsync = getControllerNamesAsync;
        function createController(name) {
            return callNativeSync('createController', { name: name });
        }
        native_1.createController = createController;
        function createControllerAsync(name, callback) {
            callNativeAsync('createController', { name: name }, callback);
        }
        native_1.createControllerAsync = createControllerAsync;
        function createObservableController(name) {
            return callNativeSync('createObservableController', { name: name });
        }
        native_1.createObservableController = createObservableController;
        function createObservableControllerAsync(name, callback) {
            callNativeAsync('createObservableController', { name: name }, callback);
        }
        native_1.createObservableControllerAsync = createObservableControllerAsync;
        function destroyController(id) {
            callNativeSync('destroyController', id);
        }
        native_1.destroyController = destroyController;
        function destroyControllerAsync(id, callback) {
            callNativeAsync('destroyController', id, callback);
        }
        native_1.destroyControllerAsync = destroyControllerAsync;
        function callMethod(id, name, args) {
            return callNativeSync('callMethod', { id: id, name: name, args: args });
        }
        native_1.callMethod = callMethod;
        function callMethodAsync(id, name, args, callback) {
            callNativeAsync('callMethod', { id: id, name: name, args: args }, callback);
        }
        native_1.callMethodAsync = callMethodAsync;
        function callInternalMethod(id, name, args) {
            return callNativeSync('callMethod', { id: id, name: name, args: args, internalMethod: true });
        }
        native_1.callInternalMethod = callInternalMethod;
        function callInternalMethodAsync(id, name, args, callback) {
            callNativeAsync('callMethod', { id: id, name: name, args: args, internalMethod: true }, callback);
        }
        native_1.callInternalMethodAsync = callInternalMethodAsync;
        function registerFunction(name, func) {
            // !native function registerFunction();
            registerFunction(name, func);
        }
        native_1.registerFunction = registerFunction;
        function log(type, messageType, message) {
            callNativeSync('log', { type: type, messageType: messageType, message: message });
        }
        native_1.log = log;
        function loadInternalScript(scriptName) {
            // !native function loadInternalScript();
            loadInternalScript(scriptName);
        }
        native_1.loadInternalScript = loadInternalScript;
        function callNativeAsync(name, data, callback) {
            var jsonData = JSON.stringify(data);
            var internalCallback = function (jsonResponse) {
                if (callback != null)
                    callback(new NativeResponse(jsonResponse));
            };
            native(name, jsonData, internalCallback);
        }
        native_1.callNativeAsync = callNativeAsync;
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
            if (response.type == htmlUi.NativeResponseType.Value)
                return response.value;
            if (response.type == htmlUi.NativeResponseType.Exception)
                throw response.exception;
        }
        native_1.callNativeSync = callNativeSync;
        var NativeResponse = (function () {
            function NativeResponse(jsonResponse) {
                var nativeResponse = JSON.parse(jsonResponse);
                this.type = nativeResponse.type;
                this.exception = nativeResponse.exception;
                this.value = nativeResponse.value;
            }
            NativeResponse.prototype.getValue = function () {
                if (this.type == htmlUi.NativeResponseType.Exception)
                    throw this.exception;
                if (this.type == htmlUi.NativeResponseType.Value)
                    return this.value;
                return undefined;
            };
            return NativeResponse;
        })();
        native_1.NativeResponse = NativeResponse;
    })(native = htmlUi.native || (htmlUi.native = {}));
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=htmlUi.native.js.map