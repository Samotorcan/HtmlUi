var htmlUi;
(function (htmlUi) {
    var ControllerChanges = (function () {
        function ControllerChanges() {
            this.changes = [];
        }
        Object.defineProperty(ControllerChanges.prototype, "hasChanges", {
            get: function () {
                return _.any(this.changes, function (change) {
                    return change.hasChanges;
                });
            },
            enumerable: true,
            configurable: true
        });
        ControllerChanges.prototype.getChange = function (id) {
            var change = _.find(this.changes, function (change) {
                change.id == id;
            });
            if (change == null) {
                change = new ControllerChange(id);
                this.changes.push(change);
            }
            return change;
        };
        ControllerChanges.prototype.clear = function () {
            this.changes = [];
        };
        return ControllerChanges;
    })();
    htmlUi.ControllerChanges = ControllerChanges;
    var ControllerChange = (function () {
        function ControllerChange(id) {
            this.id = id;
            this.properties = {};
            this.observableCollections = {};
        }
        Object.defineProperty(ControllerChange.prototype, "hasChanges", {
            get: function () {
                return _.keys(this.properties).length != 0 || (_.keys(this.observableCollections).length != 0 && _.all(this.observableCollections, function (changes) {
                    return changes.actions.length > 0;
                }));
            },
            enumerable: true,
            configurable: true
        });
        return ControllerChange;
    })();
    htmlUi.ControllerChange = ControllerChange;
    (function (ObservableCollectionChangeAction) {
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Add"] = 1] = "Add";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Remove"] = 2] = "Remove";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Replace"] = 3] = "Replace";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Move"] = 4] = "Move";
    })(htmlUi.ObservableCollectionChangeAction || (htmlUi.ObservableCollectionChangeAction = {}));
    var ObservableCollectionChangeAction = htmlUi.ObservableCollectionChangeAction;
    (function (NativeResponseType) {
        NativeResponseType[NativeResponseType["Value"] = 1] = "Value";
        NativeResponseType[NativeResponseType["Undefined"] = 2] = "Undefined";
        NativeResponseType[NativeResponseType["Exception"] = 3] = "Exception";
    })(htmlUi.NativeResponseType || (htmlUi.NativeResponseType = {}));
    var NativeResponseType = htmlUi.NativeResponseType;
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.definitions.js.map