using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class RecoveryCodes : ComponentBase
    {
        [Inject]
        public ISnackbar? SnackBar { get; set; }
        
        [Inject]
        private IApiClient? ApiClient { get; set; }

        
        private readonly List<string> _recoveryCodes = [];
        private int _recoveryCodesCount;

        protected override async Task OnInitializedAsync()
        {
            if (ApiClient == null) return;
            try
            {
               _recoveryCodesCount = await ApiClient.CountActiveRecoveryCodesEndpointAsync();
   
            }
            catch (Exception ex)
            {
                SnackBar?.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }


        /// <summary>
        /// Generate new recovery codes 
        /// </summary>
        /// <returns></returns>
        private async Task GenerateRecoveryCodesAsync()
        {
            if (ApiClient == null) return;
            try
            {
          
                var codes = await ApiClient.GenerateRecoverCodesEndpointAsync();
                
                _recoveryCodes.Clear();
                _recoveryCodes.AddRange(codes);
                _recoveryCodesCount = _recoveryCodes.Count;
            }
            catch(Exception ex)
            {
                SnackBar?.Add(ex.Message, Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }
    }
}
