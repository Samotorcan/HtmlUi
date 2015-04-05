/// <reference path="references.ts" />

module htmlUi {
    // constants
    declare var _nativeRequestUrl: string;

    export var empty = function () { }; // chrome source map bug
    export var settings = (() => {
        // !inject-constants

        return {
            nativeRequestUrl: _nativeRequestUrl
        }
    })();
} 