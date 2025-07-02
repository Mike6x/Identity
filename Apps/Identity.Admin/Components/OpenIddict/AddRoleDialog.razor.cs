using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class AddRoleDialog : ComponentBase
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Inject]
        public ISnackbar? SnackBar { get; set; }

        [Inject]
        protected IApiClient? ApiClient { get; set; }  

        [Parameter]
        public string? Owner { get; set; }
       
        [Parameter]
        public IEnumerable<RoleDto>? ExistingRoles { get; set; }

        private string? _error;
        private RoleDto? _selectedOption;

        /// <summary>
        /// Add role to user and close the dialog
        /// </summary>
        private Task AddRoleAsync()
        {
            if (null == _selectedOption) return Task.CompletedTask;
            
            if (ExistingRoles != null && !ExistingRoles.Any(u => u.Name.Equals(_selectedOption.Name)))
            {
                // var result = await ApiClient.AssignRolesToUserEndpointAsync(Owner, new AssignUserRoleCommand()
                // {
                //
                //
                // };
                // new[] { selectedOption });
                // if (result.IsSuccess)
                // {
                //     MudDialog.Close(DialogResult.Ok<RoleDto>(selectedOption));
                //     return;
                // }
                // error = result.ToString();
                return Task.CompletedTask;
            }                
            _error = $"{_selectedOption.Name} role is already assigned to user.";

            return Task.CompletedTask;
        }


        private void Cancel() => MudDialog.Cancel();
        
        private async Task<IEnumerable<RoleDto>?> SearchRoles(string value, CancellationToken ct)
        {
            // if text is null or empty, don't return values (drop-down will not open)
            if (string.IsNullOrEmpty(value) || ApiClient == null)  return null;
            
            var result = await ApiClient?.SearchRolesEndpointAsync(new SearchRolesRequest
            {
                RoleFilter = value
            })!;
            
            return result.Items;
        }
    }
}
