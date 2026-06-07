export var Microsoft;
(function (Microsoft) {
    var FluentUI;
    (function (FluentUI) {
        var Blazor;
        (function (Blazor) {
            var Select;
            (function (Select) {
                function ClearValue(id) {
                    const element = document.getElementById(id);
                    if (element) {
                        element.value = null;
                    }
                }
                Select.ClearValue = ClearValue;
                function SetComboBoxValue(id, value) {
                    const element = document.getElementById(id);
                    if (element && element.tagName === 'FLUENT-DROPDOWN' && element._control) {
                        element._control.value = value;
                    }
                }
                Select.SetComboBoxValue = SetComboBoxValue;
            })(Select = Blazor.Select || (Blazor.Select = {}));
        })(Blazor = FluentUI.Blazor || (FluentUI.Blazor = {}));
    })(FluentUI = Microsoft.FluentUI || (Microsoft.FluentUI = {}));
})(Microsoft || (Microsoft = {}));
