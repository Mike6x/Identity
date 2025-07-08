using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Scopes;

public partial class ScopeDetails : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    public required IDialogService Dialog { get; set; }
    
    [Inject]
    public required IApiClient ApiClient { get; set; } 

    [Parameter]
    public required  string Id { get; set; }
    
    private ScopeDto _model = new();
    
    private bool _canEditItems;
    
    private bool _canSearchItems;
    private string _searchString = string.Empty;
    
    private bool _loaded;
    
    protected override async Task OnParametersSetAsync()
    {
        if (AuthState == null || string.IsNullOrEmpty(Id)) return;
        
        var state = await AuthState;
        if (AuthService != null)
        {
            _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Scopes);
            _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.Scopes);
        }
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetScopeEndpointAsync(Id), Toast, Navigation)
            is { } result)
        {
            _model = result;
        }

        _loaded = true;
    }
    
    private string Title => $"{_model.Name} scope in details";
    private string Description => $"Manage {_model.Name} scope resources";
    
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(_model.Id)) return;
        
        var request = new UpdateScopeCommand
        {
            Id = _model.Id,
            Name = _model.Name,
            Description = _model.Description,
            DisplayName = _model.DisplayName,
            Resources = _model.Resources ?? [],
        };


        await ApiHelper.ExecuteCallGuardedAsync(() => ApiClient.UpdateScopeEndpointAsync(request), Toast,
                                                successMessage: "updated scope resources");

        Navigation.NavigateTo("/identity/scopes");
    }
}