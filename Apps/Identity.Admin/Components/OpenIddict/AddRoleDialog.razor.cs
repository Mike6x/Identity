using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class AddRoleDialog : ComponentBase
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }
        
        [Parameter]
        public IApiClient? ApiClient { get; set; }  

        [Parameter]
        public string? Owner { get; set; }
       
        [Parameter]
        public IEnumerable<RoleSummaryDto>? ExistingRoles { get; set; }

        private string? _error;
        private RoleSummaryDto? _selectedOption;


        private async Task AddRoleAsync()
        {
            if (ApiClient == null || string.IsNullOrEmpty(Owner) || _selectedOption == null || ExistingRoles == null) return;
            
            if (!ExistingRoles.Any(u => u.Name.Equals(_selectedOption.Name)))
            {
                var request = new AssignUserRoleCommand {
                    UserRoles = [ _selectedOption
                        // new UserRoleDetail {
                        //     RoleId = _selectedOption.Id,
                        //     RoleName = _selectedOption.Name,
                        //     Description = _selectedOption.Description,
                        //     Enabled = true
                        // }
                    ]
                };

                var result = await ApiClient.AssignRolesToUserEndpointAsync(Owner, request);
                
                if(result)
                {
                    MudDialog.Close(DialogResult.Ok(_selectedOption));
                    return;
                }
                _error = "internal error";
            }  
                
            _error = $"{_selectedOption.Name} role is already assigned to user.";
        }

        private async Task<IEnumerable<RoleSummaryDto>?> SearchRoles(string value, CancellationToken cancellationToken)
        {
            // if a text is null or empty, don't return values (drop-down will not open)
            if (string.IsNullOrEmpty(value) || ApiClient == null) return null;
            
            var result = await ApiClient.SearchRolesEndpointAsync(new SearchRolesRequest
            {
                RoleFilter = value
            }, cancellationToken);
            
            return result.Items;
        }
    }
}
