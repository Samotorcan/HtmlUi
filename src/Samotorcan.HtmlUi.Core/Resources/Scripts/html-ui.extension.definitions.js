var htmlUi;
(function (htmlUi) {
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