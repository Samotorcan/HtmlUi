var htmlUi;
(function (htmlUi) {
    var utility;
    (function (utility) {
        function argumentsToArray(args) {
            var argsArray = [];
            _.forEach(args, function (arg) {
                argsArray.push(arg);
            });
            return argsArray;
        }
        utility.argumentsToArray = argumentsToArray;
        function inject(func, inject) {
            var _this = this;
            return function () {
                inject.apply(_this, arguments);
                return func.apply(_this, arguments);
            };
        }
        utility.inject = inject;
        function shallowCopyCollection(collection) {
            if (collection == null)
                return null;
            var newCollection = [];
            _.forEach(collection, function (value, index) {
                newCollection[index] = value;
            });
            return newCollection;
        }
        utility.shallowCopyCollection = shallowCopyCollection;
        function isArrayShallowEqual(firstArray, secondArray) {
            if (firstArray === secondArray)
                return true;
            if (firstArray == null || secondArray == null)
                return false;
            if (firstArray.length != secondArray.length)
                return false;
            return !_.any(firstArray, function (value, index) {
                return value !== secondArray[index];
            });
        }
        utility.isArrayShallowEqual = isArrayShallowEqual;
    })(utility = htmlUi.utility || (htmlUi.utility = {}));
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.utility.js.map