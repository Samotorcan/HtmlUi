/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    var settings;
    (function (settings) {
        var injectConstants = (function () {
            // !inject-constants
            return {
                nativeRequestUrl: _nativeRequestUrl
            };
        })();
        settings.nativeRequestUrl = injectConstants.nativeRequestUrl;
    })(settings = htmlUi.settings || (htmlUi.settings = {}));
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.settings.js.map