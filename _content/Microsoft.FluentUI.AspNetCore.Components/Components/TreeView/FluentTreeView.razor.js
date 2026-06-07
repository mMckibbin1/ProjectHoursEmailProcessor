export var Microsoft;
(function (Microsoft) {
    var FluentUI;
    (function (FluentUI) {
        var Blazor;
        (function (Blazor) {
            var TreeView;
            (function (TreeView) {
                function Initialize(id, multiple) {
                    const treeView = document.getElementById(id);
                    if (treeView && multiple) {
                        treeView.addEventListener('keydown', (event) => {
                            if (event.code === 'Space' && event.target instanceof HTMLElement) {
                                const checkbox = event.target.querySelector('fluent-checkbox');
                                if (checkbox) {
                                    checkbox.click();
                                }
                            }
                        });
                    }
                }
                TreeView.Initialize = Initialize;
            })(TreeView = Blazor.TreeView || (Blazor.TreeView = {}));
        })(Blazor = FluentUI.Blazor || (FluentUI.Blazor = {}));
    })(FluentUI = Microsoft.FluentUI || (Microsoft.FluentUI = {}));
})(Microsoft || (Microsoft = {}));
