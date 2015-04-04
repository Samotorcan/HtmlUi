/// <reference path="references.ts" />

module htmlUi.settings {
    declare var _nativeRequestUrl: string;

    var injectConstants = (() => {
        // !inject-constants

        return {
            nativeRequestUrl: _nativeRequestUrl
        }
    })();

    export var nativeRequestUrl = injectConstants.nativeRequestUrl;
} 