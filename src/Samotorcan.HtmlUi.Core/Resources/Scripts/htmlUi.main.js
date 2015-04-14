/// <reference path="references.ts" />
// run
var htmlUi;
(function (htmlUi) {
    htmlUi.native.loadInternalScript('lodash.min.js');
    htmlUi._ = window['_'].noConflict();
})(htmlUi || (htmlUi = {}));
// definitions
var htmlUi;
(function (htmlUi) {
    (function (ObservableCollectionChangeAction) {
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Add"] = 1] = "Add";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Remove"] = 2] = "Remove";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Replace"] = 3] = "Replace";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Move"] = 4] = "Move";
    })(htmlUi.ObservableCollectionChangeAction || (htmlUi.ObservableCollectionChangeAction = {}));
    var ObservableCollectionChangeAction = htmlUi.ObservableCollectionChangeAction;
    (function (NativeResponseType) {
        NativeResponseType[NativeResponseType["Value"] = 1] = "Value";
        NativeResponseType[NativeResponseType["Undefined"] = 2] = "Undefined";
        NativeResponseType[NativeResponseType["Exception"] = 3] = "Exception";
    })(htmlUi.NativeResponseType || (htmlUi.NativeResponseType = {}));
    var NativeResponseType = htmlUi.NativeResponseType;
})(htmlUi || (htmlUi = {}));
// utility
var htmlUi;
(function (htmlUi) {
    var utility;
    (function (utility) {
        function argumentsToArray(args) {
            var argsArray = [];
            htmlUi._.forEach(args, function (arg) {
                argsArray.push(arg);
            });
            return argsArray;
        }
        utility.argumentsToArray = argumentsToArray;
        function inject(func, inject) {
            var _this = this;
            return function () {
                inject.apply(_this, arguments);
                return func.apply(_this, arguments);
            };
        }
        utility.inject = inject;
        function shallowCopyCollection(collection) {
            if (collection == null)
                return null;
            var newCollection = [];
            htmlUi._.forEach(collection, function (value, index) {
                newCollection[index] = value;
            });
            return newCollection;
        }
        utility.shallowCopyCollection = shallowCopyCollection;
        function isArrayShallowEqual(firstArray, secondArray) {
            if (firstArray === secondArray)
                return true;
            if (firstArray == null || secondArray == null)
                return false;
            if (firstArray.length != secondArray.length)
                return false;
            return !htmlUi._.any(firstArray, function (value, index) {
                return value !== secondArray[index];
            });
        }
        utility.isArrayShallowEqual = isArrayShallowEqual;
    })(utility = htmlUi.utility || (htmlUi.utility = {}));
})(htmlUi || (htmlUi = {}));
// settings
var htmlUi;
(function (htmlUi) {
    var settings;
    (function (settings) {
        var _settings = (function () {
            // !inject-constants
            return {
                nativeRequestUrl: _nativeRequestUrl
            };
        })();
        settings.nativeRequestUrl = _settings.nativeRequestUrl;
    })(settings = htmlUi.settings || (htmlUi.settings = {}));
})(htmlUi || (htmlUi = {}));
// main
var htmlUi;
(function (htmlUi) {
    function domReady(func) {
        if (document.readyState === 'complete')
            setTimeout(func, 0);
        else
            document.addEventListener("DOMContentLoaded", func);
    }
    htmlUi.domReady = domReady;
    function includeScript(scriptName, onload) {
        var scriptElement = document.createElement('script');
        document.body.appendChild(scriptElement);
        if (onload != null)
            scriptElement.onload = onload;
        scriptElement.src = scriptName;
    }
    htmlUi.includeScript = includeScript;
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=htmlUi.main.js.map