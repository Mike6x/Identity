using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Identity.Admin.Components.Pages.Roles;

public partial class RoleClaims : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    private IApiClient? ApiClient { get; set; }
    
    [Parameter]
    public string? Id { get; set; }

    private List<ClaimViewModel> _itemList = [];

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditItems;
    private bool _canSearchItems;
    
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        if (ApiClient == null) return;
        if (AuthState == null)  return;
        if(string.IsNullOrEmpty(Id)) return;
        
        var state = await AuthState;
        if (AuthService != null)
        {
            _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Users);
            _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.UserRoles);
        }
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetRoleByIdEndpointAsync(Id!), Toast, Navigation)
            is { } mainItem)
        {
            _title = $"{mainItem.Name} Claims";
            _description = $"Manage {mainItem.Name} Role Claims";

            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => ApiClient.GetRoleClaimsEndpointAsync(mainItem.Id.ToString()), Toast, Navigation)
                is { } response)
            {
                _itemList = response.ToList();
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        // var request = new AssignUserClaimCommand()
        // {
        //     UserRoles = _itemList
        // };
        //
        // Console.WriteLine($"roles : {request.UserRoles.Count}");
        //
        // await ApiHelper.ExecuteCallGuardedAsync(
        //         () => UsersClient.AssignRolesToUserEndpointAsync(Id, request),
        //         Toast,
        //         successMessage: "updated user claims");
    
        Navigation.NavigateTo("/identity/users");
    }

    private bool Search(ClaimViewModel item) =>
        string.IsNullOrWhiteSpace(_searchString)
            || item.Type?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true 
            || item.Value?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}
