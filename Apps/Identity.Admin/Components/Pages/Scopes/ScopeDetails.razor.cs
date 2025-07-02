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
    
    private ScopeDto _item = new();

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditItems;
    private bool _canSearchItems;
    
    private bool _loaded;
    
    protected override async Task OnParametersSetAsync()
    {
        if (ApiClient == null || AuthState == null || string.IsNullOrEmpty(Id)) return;
        
        var state = await AuthState;
        if (AuthService != null)
        {
            _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Scopes);
            _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.Scopes);
        }
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetScopeEndpointAsync(Id), Toast, Navigation)
            is { } scopeDto)
        {
            _item = scopeDto;
            _title = $"{_item.Name} scope in details";
            _description = $"Manage {_item.Name} scope resources";
        }

        _loaded = true;
    }
    
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(_item.Id) || ApiClient == null) return;
        
        var request = new UpdateScopeCommand
        {
            Id = _item.Id,
            Name = _item.Name,
            Description = _item.Description,
            DisplayName = _item.DisplayName,
            Resources = _item.Resources ?? [],
        };


        await ApiHelper.ExecuteCallGuardedAsync(() => ApiClient.UpdateScopeEndpointAsync(request), Toast,
                                                successMessage: "updated scope resources");

        Navigation.NavigateTo("/identity/scopes");
    }
}