using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class AddClaimDialog : ComponentBase
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Inject]
        protected IApiClient? ApiClient { get; set; }

        [Parameter]
        public string? Owner { get; set; }

        [Parameter]
        public IEnumerable<ClaimViewModel>? ExistingClaims { get; set; }

        private string? _error ;
        private ClaimViewModel _model = new ();
        
        private Task AddNewClaimAsync()
        {
            if (ExistingClaims != null && !ExistingClaims.Any(u => u.Type.Equals(_model.Type) && u.Value.Equals(_model.Value)))
            {
                //Don't try to add claim to role if role is not yet created.
                if (!string.IsNullOrEmpty(Owner))
                {
                    // var result = await Service.AddClaimAsync(Owner, model);

                    // result = await ApiClient.AddClaimToRoleEndpointAsync(Owner, model);
                    // if (result.IsSuccess)
                    // {
                    //     MudDialog.Close(DialogResult.Ok<ClaimViewModel>(model));
                    //     return;
                    // }
                    // error = result.ToString();
                    return Task.CompletedTask;
                }
             
            }
            _error = $"Claim with type {_model.Type} already exists for role.";
            return Task.CompletedTask;
        }


        private void Cancel() => MudDialog.Cancel();
    }
}
