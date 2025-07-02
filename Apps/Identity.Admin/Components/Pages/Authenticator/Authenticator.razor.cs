using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;

namespace Identity.Admin.Components.Pages.Authenticator
{
    /// <summary>
    /// Self management page for user authenticator for 2FA
    /// </summary>
    public partial class Authenticator : ComponentBase
    {
        [Inject]
        public NavigationManager? Navigator { get; set; }
        
        [Inject]
        private IApiClient? ApiClient { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            if (ApiClient == null) return;
            
            if(!await ApiClient.IsAuthenticatorEnabledEndpointAsync())
            {
                Navigator?.NavigateTo("account/authenticator/enable");
            }

            await base.OnInitializedAsync();
        }
    }
}
