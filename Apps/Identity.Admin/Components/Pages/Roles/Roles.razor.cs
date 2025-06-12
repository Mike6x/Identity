using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.EntityTable;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Identity.Admin.Components.Pages.Roles;

public partial class Roles : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    private IApiClient RolesClient { get; set; } = default!;

    protected EntityClientTableContext<RoleDto, string?, CreateOrUpdateRoleCommand> Context { get; set; } = default!;

    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canViewRoleClaims = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.RoleClaims);

        Context = new(
            entityName: "Role",
            entityNamePlural: "Roles",
            entityResource: AppResources.Roles,
            searchAction: AppActions.View,
            fields: new()
            {
                new(role => role.Name,"Name"),
                new(role => role.Description, "Description"),
                new(role => role.Id, "Id"),
            },
            idFunc: role => role.Id.ToString(),
            loadDataFunc: async () => (await RolesClient.GetRolesEndpointAsync()).ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                    || role.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await RolesClient.CreateOrUpdateRoleEndpointAsync(role),
            updateFunc: async (_, role) => await RolesClient.CreateOrUpdateRoleEndpointAsync(role),
            deleteFunc: async id => await RolesClient.DeleteRoleEndpointAsync(id!),
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => !AppRoles.IsDefault(e.Name!),
            canDeleteEntityFunc: e => !AppRoles.IsDefault(e.Name!)
            // exportAction: string.Empty
            );
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/identity/roles/{roleId}/permissions");
    }
}
