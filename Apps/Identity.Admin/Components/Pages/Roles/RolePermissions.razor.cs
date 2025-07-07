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

    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    private IApiClient? ApiClient { get; set; }
    
    [Parameter]
    public string? Id { get; set; }

    private Dictionary<string, List<PermissionViewModel>> _groupedRoleClaims =new();

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditItems;
    private bool _canSearchItems;
    
    private bool _loaded;

    static RolePermissions() => TypeAdapterConfig<AppPermission, PermissionViewModel>.NewConfig().MapToConstructor(true);

    protected override async Task OnInitializedAsync()
    {
        if (ApiClient == null) return;
        if (AuthState == null)  return;
        if(string.IsNullOrEmpty(Id)) return;
        
        var state = await AuthState;
        if (AuthService != null)
        {
            _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.RoleClaims);
            _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.RoleClaims);
        }
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetRoleByIdEndpointAsync(Id), Toast, Navigation)
            is { } mainItem)
        {
            _title = $"{mainItem.Name.ToUpper()} Permissions";
            _description = $"Manage {mainItem.Name.ToUpper()} role permissions";

            var permissions = state.User.GetTenant() == TenantConstants.Root.Id
                ? AppPermissions.All
                : AppPermissions.Admin;

            _groupedRoleClaims = permissions
                .GroupBy(p => p.Resource)
                .ToDictionary(g => g.Key, g => g.Select(p =>
                {
                    var permission = p.Adapt<PermissionViewModel>();
                    permission.Enabled = mainItem.Permissions.Contains(permission.Name);
                    return permission;
                }).ToList());
        }

        _loaded = true;
    }

    private static Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }

    private async Task SaveAsync()
    {
        if(string.IsNullOrEmpty(Id)) return;
        
        var allPermissions = _groupedRoleClaims.Values.SelectMany(a => a);
        var selectedPermissions = allPermissions.Where(a => a.Enabled);
        var request = new UpdatePermissionsCommand()
        {
            RoleId = Id,
            Permissions = selectedPermissions.Where(x => x.Enabled).Select(x => x.Name).ToList(),
        };
        await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient?.UpdateRolePermissionsEndpointAsync(request.RoleId, request)!,
                Toast,
                successMessage: "Updated Permissions.");
        Navigation.NavigateTo("/identity/roles");
    }

    private bool Search(PermissionViewModel permission) =>
        string.IsNullOrWhiteSpace(_searchString)
            || permission.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true
            || permission.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}

public record PermissionViewModel : AppPermission
{
    public bool Enabled { get; set; }

    public PermissionViewModel(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
        : base(Description, Action, Resource, IsBasic, IsRoot)
    {
    }
}
