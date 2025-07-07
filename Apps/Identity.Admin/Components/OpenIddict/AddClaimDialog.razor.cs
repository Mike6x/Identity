using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class AddClaimDialog : ComponentBase
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public IApiClient? ApiClient { get; set; }

        [Parameter]
        public string? Owner { get; set; }
        
        [Parameter]
        public IEnumerable<ClaimViewModel>? ExistingClaims { get; set; }
               
        [Parameter]
        public bool ToRole { get; set; }

        private string? _error ;
        private readonly ClaimViewModel _model = new ();
        
        private async Task AddNewClaimAsync()
        {
            if (ApiClient == null) return ;
            
            if (ExistingClaims != null && !ExistingClaims.Any(u => u.Type.Equals(_model.Type) && u.Value.Equals(_model.Value)))
            {
                //Don't try to add claim to a role if a role is not yet created.
                if (!string.IsNullOrEmpty(Owner))
                {
                    var request = new AddClaimCommand()
                    {
                        Owner = Owner,
                        ClaimToAdd = _model
                    };
                   
                    var result = ToRole 
                        ? await ApiClient.AddClaimToRoleEndpointAsync(Owner, request)
                        : await ApiClient.AddClaimToUserEndpointAsync(Owner, request);
                    if (result)
                    {
                        MudDialog.Close(DialogResult.Ok(_model));
                        return;
                    }
                    
                    _error = "internal error";
                }
            }
            
            _error = $"Claim with type {_model.Type} already exists for role.";
        }
    }
}
