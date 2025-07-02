using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Identity.Admin.Components.Pages.Users;

public partial class UserClaims : ComponentBase
{
    [Parameter]
    public string? Id { get; set; }
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = null!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IApiClient UsersClient { get; set; } = default!;

    private List<ClaimViewModel> _itemList = default!;

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditItems;
    private bool _canSearchItems;
    
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Users);
        _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.UserRoles);

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetUserEndpointAsync(Id!), Toast, Navigation)
            is { } user)
        {
            _title = $"{user.FirstName} {user.LastName}'s Claims";
            _description = $"Manage {user.FirstName} {user.LastName}'s Claims";

            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => UsersClient.GetUserClaimsEndpointAsync(user.Id.ToString()), Toast, Navigation)
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
