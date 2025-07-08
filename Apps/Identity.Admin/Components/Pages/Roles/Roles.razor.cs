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
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    public NavigationManager? Navigator { get; set; }
    
    [Inject]
    public required IApiClient ApiClient { get; set; }

    private EntityClientTableContext<RoleSummaryDto, string?, CreateOrUpdateRoleCommand>? Context { get; set; }

    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        if (AuthState == null)  return;
        
        var state = await AuthState;
        if (AuthService != null)
            _canViewRoleClaims = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.RoleClaims);

        Context = new EntityClientTableContext<RoleSummaryDto, string?, CreateOrUpdateRoleCommand>(
            entityName: "Role",
            entityNamePlural: "Roles",
            entityResource: AppResources.Roles,
            searchAction: AppActions.View,
            fields:
            [
                new EntityField<RoleSummaryDto>(role => role.Name, "Name"),
                new EntityField<RoleSummaryDto>(role => role.Description, "Description"),
                new EntityField<RoleSummaryDto>(role => role.Id, "Id")
            ],
            idFunc: role => role.Id.ToString(),
            loadDataFunc: async () => (await ApiClient.GetRolesEndpointAsync()).ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                    || role.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await ApiClient.CreateOrUpdateRoleEndpointAsync(role),
            updateFunc: async (_, role) => await ApiClient.CreateOrUpdateRoleEndpointAsync(role),
            deleteFunc: async id => await ApiClient.DeleteRoleEndpointAsync(id!),
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => !AppRoles.IsDefault(e.Name),
            canDeleteEntityFunc: e => !AppRoles.IsDefault(e.Name)
            // exportAction: string.Empty
            );
    }

    private void ToRolePermissions(in Guid  roleId) => Navigator?.NavigateTo($"/identity/roles/{roleId}/permissions");
    
    // private void ToRoleClaims(in Guid roleId) => Navigator?.NavigateTo($"/identity/roles/{roleId}/claims");
    
    private void ToRoleDetails(in Guid roleId) => Navigator?.NavigateTo($"/identity/roles/{roleId}/details");
}
