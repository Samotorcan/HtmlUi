/// <reference path="references.ts" />
// utility
var htmlUi;
(function (htmlUi) {
    var utility;
    (function (utility) {
        function inject(func, inject) {
            var _this = this;
            return function () {
                var args = [];
                for (var _i = 0; _i < arguments.length; _i++) {
                    args[_i - 0] = arguments[_i];
                }
                inject.apply(_this, args);
                return func.apply(_this, args);
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