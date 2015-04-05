/// <reference path="references.ts" />

module htmlUi {
    // constants
    declare var _nativeRequestUrl: string;

    export var settings = (() => {
        // !inject-constants

        return {
            nativeRequestUrl: _nativeRequestUrl
        }
    })();
} 