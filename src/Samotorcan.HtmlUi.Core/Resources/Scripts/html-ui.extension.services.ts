/// <reference path="references.ts" />

module htmlUi {
    var _$q: angular.IQService = null;

    export var services = {
        get $q(): angular.IQService {
            if (_$q == null)
                _$q = angular.injector(['ng']).get('$q');

            return _$q;
        }
    };
} 