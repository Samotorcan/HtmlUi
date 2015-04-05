/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    htmlUi.empty = function () {
    }; // chrome source map bug
    htmlUi.settings = (function () {
        // !inject-constants
        return {
            nativeRequestUrl: _nativeRequestUrl
        };
    })();
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.settings.js.map