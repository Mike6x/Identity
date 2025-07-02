using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.EntityTable;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Applications;

public partial class Applications : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    private IApiClient? ApiClient { get; set; }
    
    [Inject]
    public NavigationManager? Navigator { get; set; }
    
    public required IDialogService Dialog;
    
    private bool _canViewItemDetails;
    
    private EntityClientTableContext<ApplicationDto, string?, ApplicationViewModel> Context { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (ApiClient == null) return;
        if (AuthState == null)  return;
        
        var state = await AuthState;

        if (AuthService != null)
            _canViewItemDetails =
                await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.Clients);

        Context = new EntityClientTableContext<ApplicationDto, string?, ApplicationViewModel>(
            entityName: "Application",
            entityNamePlural: "Applications",
            entityResource: AppResources.Clients,
            searchAction: AppActions.View,
            fields:
            [
                new EntityField<ApplicationDto>(item => item.ClientId, "ClientId"),
                new EntityField<ApplicationDto>(item => item.DisplayName, "Display Name"),
                new EntityField<ApplicationDto>(item => item.ClientType, "Client Type"),
                new EntityField<ApplicationDto>(item => item.ConsentType, "Consent Type"),
        
        
                new EntityField<ApplicationDto>(item => item.Id, "Internal Id"),
                new EntityField<ApplicationDto>(item => item.ApplicationType, "App Type"),
                new EntityField<ApplicationDto>(item => item.IsConfidentialClient, "IsConfidential", Type: typeof(bool))
            ],
            idFunc: item => item.Id.ToString(),
            loadDataFunc: async () => (await ApiClient.GetApplicationsEndpointAsync()).ToList(),
            searchFunc: (searchString, item) =>
                string.IsNullOrWhiteSpace(searchString)
                    || item.ClientId.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || item.DisplayName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async item =>
            {
                var request = item.Adapt<CreateClientCommand>();
                await ApiClient.CreateApplicationEndpointAsync(request);
            },
            updateFunc: async (_, item) => await ApiClient.UpdateApplicationEndpointAsync(item.Adapt<UpdateClientCommand>()),
            deleteFunc: async id => await ApiClient.DeleteApplicationEndpointAsync(id!),
            hasExtraActionsFunc: () => _canViewItemDetails

            );
    }
    
    protected void ManageDetails(ApplicationDto client) => Navigator?.NavigateTo($"/identity/Applications/{client.Id}/details");
    
}

public class ApplicationViewModel : UpdateClientCommand
{

    
}