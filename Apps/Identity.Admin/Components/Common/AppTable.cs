// using MediatR.Courier;

using Identity.Admin.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.Common;

public class AppTable<T> : MudTable<T>
{
    [Inject]
    private IClientPreferenceManager ClientPreferences { get; set; } = default!;
    // [Inject]
    // protected ICourier Courier { get; set; } = default!;


    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference clientPreference)
        {
            SetTablePreference(clientPreference.TablePreference);
        }

        // Courier.SubscribeWeak<NotificationWrapper<AppTablePreference>>(wrapper =>
        // {
        //     SetTablePreference(wrapper.Notification);
        //     StateHasChanged();
        // });
        
 await base.OnInitializedAsync();
    }

    private void SetTablePreference(AppTablePreference tablePreference)
    {
        Dense = tablePreference.IsDense;
        Striped = tablePreference.IsStriped;
        Bordered = tablePreference.HasBorder;
        Hover = tablePreference.IsHoverable;
    }
}
