using Client.Infrastructure.Api;
using Identity.Admin.Components.OpenIddict.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Identity.Admin.Components.OpenIddict
{
    /// <summary>
    /// ResetAuthenticator component allows user to reset the authenticator key.
    /// 2FA is disabled after this operation and user needs to re-enable 2FA and
    /// verify his authenticator app.
    /// </summary>
    public partial class ResetAuthenticator : ComponentBase
    {
        [Inject]
        public NavigationManager? Navigator { get; set; }
        
        [Inject]
        private IApiClient? ApiClient { get; set; }

        private readonly ResetAuthenticatorViewModel _model = new();

        /// <summary>
        /// Disable 2FA for user account and reset Authenticator key.      
        /// </summary>
        private async void ResetAuthenticatorAsync()
        {
            // var result = await ApiClient.ResetAuthenticatorEndpointAsync(_model.Code);
            // if (!result.IsSuccess)
            // {
            //     SnackBar.Add(result.ToString(), Severity.Error, config =>
            //     {
            //         config.ShowCloseIcon = true;
            //         config.RequireInteraction = true;
            //     });
            //     return;
            // }
            
            if (ApiClient == null) return;
            await ApiClient.ResetAuthenticatorEndpointAsync(_model.Code);
            
            await DialogService.ShowMessageBox("Success",
                "Authenticator is reset now. You need to re-configure authenticator again to enable 2FA!");
          
            Navigator?.NavigateTo("account/authenticator/enable");
        }
    }
}
