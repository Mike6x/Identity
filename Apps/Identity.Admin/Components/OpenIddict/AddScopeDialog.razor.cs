using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class AddScopeDialog : ComponentBase
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar? SnackBar { get; set; }
        
        [Inject]
        protected IApiClient? ApiClient { get; set; }

        
        private ScopeSummaryDto? _selectedOption;
        private void AddScope()
        {
            if (_selectedOption != null)
            {
                MudDialog.Close(DialogResult.Ok(_selectedOption));
            }
        }

        private void Cancel() => MudDialog.Cancel();
        
        private async Task<IEnumerable<ScopeSummaryDto>?> SearchScopes(string value, CancellationToken cancellationToken)
        {
            // if text is null or empty, don't return values (drop-down will not open)
            if (string.IsNullOrEmpty(value) || ApiClient == null) return null;
            
            var result = await ApiClient.SearchScopesEndpointAsync(new SearchScopesRequest
            {
                ScopesFilter = value
            }, cancellationToken);
            
            return result.Items;
        }
    }
}
