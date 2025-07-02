using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.EntityTable;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Scopes;

public partial class Scopes : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    public NavigationManager? Navigator { get; set; }
    
    [Inject]
    private IApiClient? ApiClient { get; set; }
    
    public required IDialogService Dialog;
    
    private bool _canViewScopeResources;
    private EntityClientTableContext<ScopeDto, string?, ScopeViewModel> Context { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (ApiClient == null) return;
        if (AuthState == null)  return;
        
        var state = await AuthState;

        if (AuthService != null)
            _canViewScopeResources =
                await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.Scopes);

        Context = new EntityClientTableContext<ScopeDto, string?, ScopeViewModel>(
            entityName: "Scope",
            entityNamePlural: "Scopes",
            entityResource: AppResources.Scopes,
            searchAction: AppActions.View,
            fields:
            [
                new EntityField<ScopeDto>(item => item.Name, "Name"),
                new EntityField<ScopeDto>(item => item.DisplayName, "Display Name"),
                new EntityField<ScopeDto>(item => item.Description, "Description"),
                // new EntityField<ScopeDto>(item => item.Resources?.ToString(), "Resource"),
                new EntityField<ScopeDto>(item => item.Id, "Id")
            ],
            idFunc: scope => scope.Id.ToString(),
            loadDataFunc: async () => (await ApiClient.GetScopesEndpointAsync()).ToList(),
            searchFunc: (searchString, scope) =>
                string.IsNullOrWhiteSpace(searchString)
                    || scope.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || scope.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async scope => await ApiClient.CreateScopeEndpointAsync(scope.Adapt<CreateScopeCommand>()),
            updateFunc: async (_, scope) => await ApiClient.UpdateScopeEndpointAsync(scope.Adapt<UpdateScopeCommand>()),
            deleteFunc: async id => await ApiClient.DeleteScopeEndpointAsync(id!),
            hasExtraActionsFunc: () => _canViewScopeResources
            // canUpdateEntityFunc: e => !AppScopes.IsDefault(e.Name),
            // canDeleteEntityFunc: e => !AppScopes.IsDefault(e.Name)
            // exportAction: string.Empty
            );
    }
    
    
    protected void ManageResources(ScopeDto scope) => Navigator?.NavigateTo($"/identity/scopes/{scope.Id}/details");
    
}

public class ScopeViewModel : UpdateScopeCommand
{

}