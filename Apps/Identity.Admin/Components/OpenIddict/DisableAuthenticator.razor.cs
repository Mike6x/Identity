using Client.Infrastructure.Api;
using Identity.Admin.Components.OpenIddict.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    /// <summary>
    /// DisableAuthenticator component allows disabling the 2FA authentication for user account.
    /// </summary>
    public partial class DisableAuthenticator : ComponentBase
    {

        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar? SnackBar { get; set; }

        [Inject]
        public required NavigationManager Navigator { get; set; }
        
        [Inject]
        protected IApiClient? ApiClient { get; set; }
        

        private DisableAuthenticatorViewModel _model  = new();

        /// <summary>
        /// Disable 2FA for user account
        /// </summary>
        private async void DisableAuthenticatorAsync()
        {
            if (ApiClient != null) await ApiClient.DisableAuthenticatorEndpointAsync(_model.Code);
            // if (!result.IsSuccess)
            // {
            //     SnackBar.Add(result.ToString(), Severity.Error, config =>
            //     {
            //         config.ShowCloseIcon = true;
            //         config.RequireInteraction = true;
            //     });
            //     return;
            // }

            await DialogService.ShowMessageBox("Success",
              "2FA is disabled now. You should enable 2FA for a better security of your account.");

            Navigator.NavigateTo("account/authenticator/enable");
        }
    }
}
