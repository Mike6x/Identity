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
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
        
    [Inject]
    public  required IDialogService Dialog { get; set; }
        
    [Inject]
    public required IApiClient ApiClient { get; set; } 

    [Parameter]
    public required  string Id { get; set; }

    private ApplicationDto _model = new();

    private string Title => $"{_model.ClientId} application";
    private string Description => $"Manage {_model.ClientId} application details";

    private string _searchString = string.Empty;

    private bool _canEditItems;
    private bool _canSearchItems;
    
    private bool _loaded;
    
    protected override async Task OnParametersSetAsync()
    {
        if ( AuthState == null || string.IsNullOrEmpty(Id) ) return;
        
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
            _model = applicationDto;
 
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        if ( string.IsNullOrEmpty(_model.Id) ) return;

        var request = _model.Adapt<UpdateClientCommand>();
        
        await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.UpdateApplicationEndpointAsync(request), 
            Toast, 
            successMessage: "updated client successfully");

        Navigation.NavigateTo("/identity/applications");
    }
}