using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Roles;

public partial class RolePermissions
{
    [Parameter]
    public string Id { get; set; } = default!; // from route
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IApiClient RolesClient { get; set; } = default!;

    private Dictionary<string, List<PermissionViewModel>> _groupedRoleClaims = default!;

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;

    static RolePermissions() => TypeAdapterConfig<AppPermission, PermissionViewModel>.NewConfig().MapToConstructor(true);

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _canEditRoleClaims = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.RoleClaims);
        _canSearchRoleClaims = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.RoleClaims);

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.GetRolePermissionsEndpointAsync(Id), Toast, Navigation)
            is RoleDto role && role.Permissions is not null)
        {
            _title = $"{role.Name} Permissions";
            _description = $"Manage {role.Name} Role Permissions";

            var permissions = state.User.GetTenant() == TenantConstants.Root.Id
                ? AppPermissions.All
                : AppPermissions.Admin;

            _groupedRoleClaims = permissions
                .GroupBy(p => p.Resource)
                .ToDictionary(g => g.Key, g => g.Select(p =>
                {
                    var permission = p.Adapt<PermissionViewModel>();
                    permission.Enabled = role.Permissions.Contains(permission.Name);
                    return permission;
                }).ToList());
        }

        _loaded = true;
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }

    private async Task SaveAsync()
    {
        var allPermissions = _groupedRoleClaims.Values.SelectMany(a => a);
        var selectedPermissions = allPermissions.Where(a => a.Enabled);
        var request = new UpdatePermissionsCommand()
        {
            RoleId = Id,
            Permissions = selectedPermissions.Where(x => x.Enabled).Select(x => x.Name).ToList(),
        };
        await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.UpdateRolePermissionsEndpointAsync(request.RoleId, request),
                Toast,
                successMessage: "Updated Permissions.");
        Navigation.NavigateTo("/identity/roles");
    }

    private bool Search(PermissionViewModel permission) =>
        string.IsNullOrWhiteSpace(_searchString)
            || permission.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true
            || permission.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}

public record PermissionViewModel(
    string Description,
    string Action,
    string Resource,
    bool IsBasic = false,
    bool IsRoot = false)
    : AppPermission(Description, Action, Resource, IsBasic, IsRoot)
{
    public bool Enabled { get; set; }
}
