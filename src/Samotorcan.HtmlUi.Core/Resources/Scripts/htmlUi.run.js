/// <reference path="references.ts" />
// run
var htmlUi;
(function (htmlUi) {
    htmlUi.native.loadInternalScript('lodash.min.js');
    htmlUi._ = window['_'].noConflict();
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=htmlUi.run.js.map