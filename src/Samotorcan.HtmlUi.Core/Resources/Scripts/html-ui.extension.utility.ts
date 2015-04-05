module htmlUi.utility {
    export function argumentsToArray(args: IArguments): any[] {
        var argsArray = [];

        _.forEach(args, function (arg) {
            argsArray.push(arg);
        });

        return argsArray;
    }

    export function inject(func: Function, inject: Function): Function {
        return () => {
            inject.apply(this, arguments);
            return func.apply(this, arguments);
        };
    }

    export function shallowCopyCollection<T>(collection: T[]): T[] {
        if (collection == null)
            return null;

        var newCollection: T[] = [];
        _.forEach(collection, function (value, index) {
            newCollection[index] = value;
        });

        return newCollection;
    }

    export function isArrayShallowEqual<T>(firstArray: T[], secondArray: T[]): boolean {
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
}