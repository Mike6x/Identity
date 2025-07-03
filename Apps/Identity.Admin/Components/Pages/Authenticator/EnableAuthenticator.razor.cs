using Client.Infrastructure.Api;
using Identity.Admin.Components.OpenIddict.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Authenticator
{
    /// <summary>
    /// Component for setting up the authenticator for 2FA
    /// </summary>
    public partial class EnableAuthenticator : ComponentBase, IAsyncDisposable
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public NavigationManager? Navigator { get; set; }

        [Inject]
        public ISnackbar? SnackBar { get; set; }
        
                
        [Inject]
        private IApiClient? ApiClient { get; set; }
        
        public required IDialogService Dialog;

        EnableAuthenticatorViewModel model = new();
        IJSObjectReference? module;

        protected override async Task OnInitializedAsync()
        {
            if (ApiClient == null) return;
            
            try
            {
                var result = await ApiClient.GetAuthenticatorConfigEndpointAsync();
                
                model.SharedKey = result.SharedKey;
                model.AuthenticatorUri = result.AuthenticatorUri;
            }
            catch (Exception ex)
            {
                SnackBar?.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/Authenticator/EnableAuthenticator.razor.js");               
            }
            await GenerateQrCodeAsync();

        }

        /// <summary>
        /// Enable the authenticator once user has completed the required setup steps
        /// </summary>
        /// <returns></returns>
        private async Task EnableAuthenticatorAsync()
        {
            if (ApiClient == null) return;
  
           await ApiClient.EnableAuthenticatorEndpointAsync(model.Code);
            
            // if (!result.IsSuccess)
            // {
            //     SnackBar.Add(result.ToString(), Severity.Error, config =>
            //     {
            //         config.ShowCloseIcon = true;
            //         config.RequireInteraction = true;
            //     });
            //     return;
            // }
            
            await DialogService.ShowMessageBox("Success", "2FA is enabled and your account is more secure now. ");
            Navigator?.NavigateTo("account/authenticator/manage");
        }

        private async Task GenerateQrCodeAsync()
        {
            if(module is not null)
            {
                await module.InvokeVoidAsync("generateQrCode");
                return;
            }
            await Task.CompletedTask;
        }        

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
    }
}
