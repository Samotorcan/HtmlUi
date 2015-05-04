/// <reference path="references.ts" />

// run
module htmlUi {
    native.loadInternalScript('lodash.min.js');

    export var _: _.LoDashStatic = window['_'].noConflict();
}