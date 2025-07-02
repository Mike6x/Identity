using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class AddResourceDialog : ComponentBase
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public required IEnumerable<string> ExistingResources { get; set; }
        
        private string? _error;
        private string _resource = string.Empty;
        private void AddNewResource()
        {
            if (!ExistingResources.Any(u => u.Equals(_resource)))
            {
                MudDialog.Close(DialogResult.Ok(_resource));
                return;
            }
            _error = "Resource is already added.";
        }
    }
}
