using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.OpenIddict;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Roles
{
    public partial class RoleDetails : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState>? AuthState { get; set; }
        [Inject]
        protected IAuthorizationService? AuthService { get; set; }
        
        [Inject]
        public required IDialogService Dialog { get; set; }
        
        [Inject]
        public required IApiClient ApiClient { get; set; } 
    
        [Parameter]
        public required string Id { get; set; }
        
        private RoleDto _model = new();
        
        private bool _canEditItems;
        
        private bool _canSearchItems;
        private string _searchString = string.Empty;
        
        private bool _itemUpdated;
        private string _roleName = string.Empty;
        private string? _roleDescription = string.Empty;
 
        private bool _loaded;
        
        protected override async Task OnInitializedAsync()
        {
            if (AuthState == null || string.IsNullOrEmpty(Id)) return;
        
            var state = await AuthState;
            if (AuthService != null)
            {
                _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Users);
                _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.UserRoles);
            }
        
            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => ApiClient.GetRoleByIdEndpointAsync(Id), Toast, Navigation)
                is { } result)
            {
                _model = result;
                
                _roleName = _model.Name;
                _roleDescription = _model.Description;
            }

            _loaded = true;
        }
        
        private string Title  => $"{_model.Name} ROLE";
        private string Description => $"Manage {_model.Name} role in details";

        private void ToggleEditRoleName() => _itemUpdated = !_itemUpdated;
        private bool RoleChanged => _roleName.Equals(_model.Name) && (_roleDescription?.Equals(_model.Description) ?? true);
        private async Task UpdateRoleNameAsync()
        {
            var request = new CreateOrUpdateRoleCommand
            {
                Id = _model.Id.ToString(),
                Name = _model.Name,
                Description = _model.Description
            };
            
            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => ApiClient.CreateOrUpdateRoleEndpointAsync(request), Toast, Navigation, null, "Item was updated")
                is not null)
            {
                _roleName = _model.Name;
                _roleDescription = _model.Description;
                ToggleEditRoleName();
            }
            else
            {
                Toast.Add($"Internal error.", Severity.Error);
            }
        }
        
        private async Task AddClaimAsync()
        {
            var parameters = new DialogParameters
            {
                { "Owner", _model.Name },
                { "ExistingClaims", _model.Claims },
                { "ApiClient", ApiClient },
                { "ToRole", true}
            };
            
            var dialog = await Dialog.ShowAsync<AddClaimDialog>("Add Claim", parameters, new DialogOptions { MaxWidth = MaxWidth.ExtraLarge, CloseButton = true });
            
            var result = await dialog.Result;
            if (result is { Canceled: false, Data: ClaimViewModel claim })
            {
                _model.Claims.Add(claim);
                Toast.Add($"Claim was added.", Severity.Success);
            }
        }

        private async Task RemoveClaimAsync(ClaimViewModel claim)
        {
            if (_model.Claims.Contains(claim))
            {
                var request = new RemoveClaimCommand
                {
                    Owner = _model.Name,
                    ClaimToRemove = claim
                };
                
                if (await ApiHelper.ExecuteCallGuardedAsync(
                        () => ApiClient.RemoveClaimOfRoleEndpointAsync(_model.Name, request), 
                                        Toast, Navigation, null, $"Claim {claim.Type}:{claim.Value} was removed.")
                    is { } result)
                {
                    _model.Claims.Remove(claim);
                }
                
                if(!result) Toast.Add($"Failed to delete claim {claim.Type}:{claim.Value}.", Severity.Error);
            }
        }
        
        private async Task<bool> UpdateClaimAsync(ClaimViewModel original, ClaimViewModel modified)
        {

            var request = new ChangeClaimCommand
            {
                Owner = _model.Name,
                Original = original,
                Modified = modified
            };
                
            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => ApiClient.ChangeClaimOfRoleEndpointAsync(_model.Name, request), 
                    Toast, Navigation, null, $"Claim was updated."))
            {
                return true;
            }
            
            Toast.Add($"Internal error.", Severity.Error);
            return false;
        }
    }
}
