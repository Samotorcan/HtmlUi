/// <reference path="references.ts" />
var htmlUi;
(function (htmlUi) {
    var _$q = null;
    htmlUi.services = {
        get $q() {
            if (_$q == null)
                _$q = angular.injector(['ng']).get('$q');
            return _$q;
        }
    };
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.services.js.map