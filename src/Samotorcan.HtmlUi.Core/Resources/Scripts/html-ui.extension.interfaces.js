var htmlUi;
(function (htmlUi) {
    (function (ObservableCollectionChangeAction) {
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Add"] = 1] = "Add";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Remove"] = 2] = "Remove";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Replace"] = 3] = "Replace";
        ObservableCollectionChangeAction[ObservableCollectionChangeAction["Move"] = 4] = "Move";
    })(htmlUi.ObservableCollectionChangeAction || (htmlUi.ObservableCollectionChangeAction = {}));
    var ObservableCollectionChangeAction = htmlUi.ObservableCollectionChangeAction;
})(htmlUi || (htmlUi = {}));
//# sourceMappingURL=html-ui.extension.interfaces.js.map