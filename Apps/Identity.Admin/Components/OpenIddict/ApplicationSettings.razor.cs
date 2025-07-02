using Identity.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict;

public partial class ApplicationSettings : ComponentBase
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public required IEnumerable<string> ConfiguredSettings { get; set; }

    private IEnumerable<string> AllSettings { get; } = new List<string>(TokenLifeTimesHelper.TokenLifeTimeNames);

    private string? _selectedSetting;
    private string? _configuredValue;
    private string? _error;

    private void AddNewSetting()
    {
        if (string.IsNullOrEmpty(_selectedSetting))
        {
            _error = "Setting is required";
            return;
        }
        if (string.IsNullOrEmpty(_configuredValue))
        {
            _error = "Value is required";
            return;
        }
        if(!TimeSpan.TryParse(_configuredValue, out _ ))
        {
            _error = "Failed to convert value to TimeSpan";
            return;
        }
        
        MudDialog.Close(DialogResult.Ok<KeyValuePair<string,string>>(new KeyValuePair<string, string>(TokenLifeTimesHelper.GetValueFromName(_selectedSetting), _configuredValue)));
        return;
    }

    private void Cancel() => MudDialog.Cancel();
}