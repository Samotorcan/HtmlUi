var htmlUi = htmlUi || {};

(function () {
    // !inject-constants
    nativeRequestUrl = nativeRequestUrl || null;

    var services = {};

    var _$q = null;
    Object.defineProperty(services, '$q', {
        get: function () {
            if (_$q == null)
                _$q = angular.injector(['ng']).get('$q');

            return _$q;
        }
    });

    htmlUi._native = {
        syncControllerChanges: function (controllers) {
            return nativeSynchronous('syncControllerChanges', controllers);
        },
        syncControllerChangesAsync: function (controllers) {
            return native('syncControllerChanges', controllers);
        },

        getControllerNames: function () {
            return nativeSynchronous('getControllerNames');
        },
        getControllerNamesAsync: function () {
            return native('getControllerNames', null);
        },

        createController: function (name, id) {
            return nativeSynchronous('createController', { name: name, id: id });
        },
        createControllerAsync: function (name, id) {
            return native('createController', { name: name, id: id });
        },

        destroyController: function (id) {
            return nativeSynchronous('destroyController', id);
        },
        destroyControllerAsync: function (id) {
            return native('destroyController', id);
        },

        callMethod: function (id, name, args) {
            return nativeSynchronous('callMethod', { id: id, name: name, args: args });
        },
        callMethodAsync: function (id, name, args) {
            return native('callMethod', { id: id, name: name, args: args });
        },

        callInternalMethod: function (id, name, args) {
            return nativeSynchronous('callMethod', { id: id, name: name, args: args, internalMethod: true });
        },
        callInternalMethodAsync: function (id, name, args) {
            return native('callMethod', { id: id, name: name, args: args, internalMethod: true });
        },

        registerFunction: function (name, func) {
            // !native function registerFunction();
            registerFunction(name, func);
        },

        log: function (type, messageType, message) {
            return nativeSynchronous('log', { type: type, messageType: messageType, message: message });
        }
    };

    function native(name, data) {
        var deferred = services.$q.defer();

        // !native function native();
        native(name, JSON.stringify(data), function (json) {
            var response = JSON.parse(json);

            if (response.type == 'value')
                deferred.resolve(response.value);
            else if (response.type == 'exception')
                deferred.reject(response.exception);
            else
                deferred.resolve();
        });

        return deferred.promise;
    };

    function nativeSynchronous(action, data) {
        var xhr = new XMLHttpRequest();
        var url = nativeRequestUrl + action;

        if (data != null) {
            xhr.open('POST', url, false);
            xhr.send(JSON.stringify(data));
        } else {
            xhr.open('GET', url, false);
            xhr.send();
        }

        var response = JSON.parse(xhr.responseText);

        if (response.type == 'value')
            return response.value;

        if (response.type == 'exception')
            throw response.exception;
    }
})();