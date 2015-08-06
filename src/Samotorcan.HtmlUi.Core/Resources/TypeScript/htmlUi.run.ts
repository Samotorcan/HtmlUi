/// <reference path="references.ts" />

// run
namespace htmlUi {
    native.loadInternalScript('lodash.min.js');

    export var _: _.LoDashStatic = window['_'].noConflict();
}