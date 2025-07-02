using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Applications;

public partial class ApplicationDetails : ComponentBase
{
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    protected IApiClient? ApiClient { get; set; }
    [Inject]
    public IDialogService? Dialog { get; set; }
    
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }

    [Parameter]
    public string? Id { get; set; }

    ApplicationDto _item = new();

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditItems;
    private bool _canSearchItems;
    
    private bool _loaded;
    
    protected override async Task OnParametersSetAsync()
    {
        if ( ApiClient == null || AuthState == null || string.IsNullOrEmpty(Id) ) return;
        
        var state = await AuthState;
        if (AuthService != null)
        {
            _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Clients);
            _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.Clients);
        }
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetApplicationEndpointAsync(Id), Toast, Navigation)
            is { } applicationDto)
        {
            _item = applicationDto;
            _title = $"{_item.ClientId} application";
            _description = $"Manage {_item.ClientId} application details";
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        if ( ApiClient == null || string.IsNullOrEmpty(_item.Id) ) return;

        var request = _item.Adapt<UpdateClientCommand>();
        
        await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.UpdateApplicationEndpointAsync(_item.Adapt<UpdateClientCommand>()), 
            Toast, 
            successMessage: "updated client successfully");

        Navigation.NavigateTo("/identity/applications");
    }
}