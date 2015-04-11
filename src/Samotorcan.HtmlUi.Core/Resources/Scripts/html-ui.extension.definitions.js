var htmlUi;
(function (htmlUi) {
    var ControllerChange = (function () {
        function ControllerChange(controllerId) {
            this.id = controllerId;
            this.properties = {};
            this.observableCollections = {};
        }
        Object.defineProperty(ControllerChange.prototype, "hasChanges", {
            get: function () {
                return _.keys(this.properties).length != 0 || (_.keys(this.observableCollections).length != 0 && _.any(this.observableCollections, function (changes) {
                    return changes.hasChanges;
                }));
            },
            enumerable: true,
            configurable: true
        });
        ControllerChange.prototype.getObservableCollection = function (propertyName) {
            var observableCollections = _.find(this.observableCollections, function (observableCollection) {
                observableCollection.name == propertyName;
            });
            if (observableCollections == null) {
                observableCollections = new ObservableCollectionChanges();
                this.observableCollections[propertyName] = observableCollections;
            }
            return observableCollections;
        };
        ControllerChange.prototype.setProperty = function (propertyName, value) {
            this.properties[propertyName] = value;
        };
        ControllerChange.prototype.getProperty = function (propertyName) {
            return this.properties[propertyName];
        };
        ControllerChange.prototype.hasProperty = function (propertyName) {
            return _.has(this.properties, propertyName);
        };
        ControllerChange.prototype.removeProperty = function (propertyName) {
            if (this.hasProperty(propertyName))
                delete this.properties[propertyName];
        };
        ControllerChange.prototype.addObservableCollectionChange = function (propertyName, action, newItem, newStartingIndex, oldStartingIndex) {
            this.getObservableCollection(propertyName).actions.push(new ObservableCollectionChange(action, newItem, newStartingIndex, oldStartingIndex));
        };
        ControllerChange.prototype.hasObservableCollection = function (propertyName) {
            return _.has(this.observableCollections, propertyName);
        };
        ControllerChange.prototype.removeObservableCollection = function (propertyName) {
            if (this.hasObservableCollection(propertyName))
                delete this.observableCollections[propertyName];
        };
        ControllerChange.prototype.clear = function () {
            this.properties = {};
            this.observableCollections = {};
        };
        return ControllerChange;
    })();
    htmlUi.ControllerChange = ControllerChange;
    var ObservableCollectionChanges = (function () {
        function ObservableCollectionChanges() {
            this.actions = [];
        }
        Object.defineProperty(ObservableCollectionChanges.prototype, "hasChanges", {
            get: function () {
                return _.keys(this.actions).length != 0;
            },
            enumerable: true,
            configurable: true
        });
        return ObservableCollectionChanges;
    })();
    htmlUi.ObservableCollectionChanges = ObservableCollectionChanges;
    var ObservableCollectionChange = (function () {
        function ObservableCollectionChange(action, newItem, newStartingIndex, oldStartingIndex) {
            this.action = action;
            this.newItems = [newItem];
            this.newStartingIndex = newStartingIndex;
            this.oldStartingIndex = oldStartingIndex;
            if (this.newItems == null)
                this.newItems = [];
        }
        return ObservableCollectionChange;
    })();
    htmlUi.ObservableCollectionChange = ObservableCollectionChange;
    (function (ObservableCollectionChangeAction) {
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Add"] = 1] = "Add";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Remove"] = 2] = "Remove";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Replace"] = 3] = "Replace";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Move"] = 4] = "Move";
    })(htmlUi.ObservableCollectionChangeAction || (htmlUi.ObservableCollectionChangeAction = {}));
    var ObservableCollectionChangeAction = htmlUi.ObservableCollectionChangeAction;
    var ControllerDataContainer = (function () {
        function ControllerDataContainer() {
            this.data = {};
            this.controllerChanges = [];
        }
        Object.defineProperty(ControllerDataContainer.prototype, "hasControllerChanges", {
            get: function () {
                return _.any(this.controllerChanges, function (controllerChange) {
                    return controllerChange.hasChanges;
                });
            },
            enumerable: true,
            configurable: true
        });
        ControllerDataContainer.prototype.addControllerData = function (controllerId) {
            var controllerData = null;
            this.data[controllerId] = controllerData = new ControllerData(controllerId);
            this.controllerChanges.push(controllerData.change);
            return controllerData;
        };
        ControllerDataContainer.prototype.getControllerData = function (controllerId) {
            return this.data[controllerId];
        };
        ControllerDataContainer.prototype.getControllerDataByScopeId = function (scopeId) {
            return _.find(this.data, function (controllerData) {
                return controllerData.scopeId == scopeId;
            });
        };
        ControllerDataContainer.prototype.clearControllerChanges = function () {
            _.forEach(this.controllerChanges, function (controllerChange) {
                controllerChange.clear();
            });
        };
        return ControllerDataContainer;
    })();
    htmlUi.ControllerDataContainer = ControllerDataContainer;
    var ControllerData = (function () {
        function ControllerData(controllerId) {
            this.controllerId = controllerId;
            this.propertyValues = {};
            this.observableCollectionValues = {};
            this.watches = {};
            this.change = new ControllerChange(controllerId);
        }
        ControllerData.prototype.hasProperty = function (propertyName) {
            return _.has(this.propertyValues, propertyName);
        };
        ControllerData.prototype.hasPropertyValue = function (propertyName, propertyValue) {
            return this.hasProperty(propertyName) && this.propertyValues[propertyName] === propertyValue;
        };
        ControllerData.prototype.setPropertyValue = function (propertyName, propertyValue) {
            this.propertyValues[propertyName] = propertyValue;
        };
        ControllerData.prototype.removePropertyValue = function (propertyName) {
            if (this.hasProperty(propertyName))
                delete this.propertyValues[propertyName];
        };
        ControllerData.prototype.hasObservableCollection = function (propertyName) {
            return _.has(this.observableCollectionValues, propertyName);
        };
        ControllerData.prototype.hasObservableCollectionValue = function (propertyName, observableCollectionValue) {
            return this.hasObservableCollection(propertyName) && htmlUi.utility.isArrayShallowEqual(this.observableCollectionValues[propertyName], observableCollectionValue);
        };
        ControllerData.prototype.setObservableCollectionValue = function (propertyName, observableCollectionValue) {
            this.observableCollectionValues[propertyName] = observableCollectionValue;
        };
        ControllerData.prototype.removeObservableCollectionValue = function (propertyName) {
            if (this.hasObservableCollection(propertyName))
                delete this.observableCollectionValues[propertyName];
        };
        ControllerData.prototype.setControllerPropertyValue = function (propertyName, propertyValue) {
            this.$scope[propertyName] = propertyValue;
        };
        ControllerData.prototype.hasWatch = function (propertyName) {
            return _.has(this.watches, propertyName);
        };
        ControllerData.prototype.removeWatch = function (propertyName) {
            if (this.hasWatch(propertyName)) {
                this.watches[propertyName]();
                delete this.watches[propertyName];
            }
        };
        ControllerData.prototype.addWatch = function (propertyName, watch) {
            this.removeWatch(propertyName);
            this.watches[propertyName] = watch;
        };
        return ControllerData;
    })();
    htmlUi.ControllerData = ControllerData;
    (function (NativeResponseType) {
        NativeResponseType[NativeResponseType["Value"] = 1] = "Value";
        NativeResponseType[NativeResponseType["Undefined"] = 2] = "Undefined";
        NativeResponseType[NativeResponseType["Exception"] = 3] = "Exception";
    })(htmlUi.NativeResponseType || (htmlUi.NativeResponseType = {}));
    var NativeResponseType = htmlUi.NativeResponseType;
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.definitions.js.map