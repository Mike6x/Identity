using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.Common;
using Identity.Admin.Components.OpenIddict;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Users
{
    public partial class EditUser : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState>? AuthState { get; set; }
        
        [Inject]
        protected IAuthorizationService? AuthService { get; set; }
   
        [Inject]
        public required IApiClient ApiClient { get; set; }
        
        [Parameter]
        public required string Id { get; set; }

        [Inject]
        public required IDialogService Dialog { get; set; }


        private UserDto _model = new();

        private CustomValidation? _customValidation;
        
        private bool _canEditItems;
        
        private bool _canSearchItems;
        private string _searchString = string.Empty;

    
        private bool _isActive;
        private char _firstLetterOfName;
        private Uri? _imageUrl;
        
        private bool _loaded;
        
        private string Title => $"{_model.FirstName} {_model.LastName}'s Profile";
        private string Description => $"Id: {_model.Id}";
        
        private DateTime? LockoutEndDate { get; set; }
        private TimeSpan? LockoutEndTime { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            if (AuthState == null || string.IsNullOrEmpty(Id)) return;
            
            var state = await AuthState;
            if (AuthService != null)
            {
                _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Users);
                _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.Search, AppResources.Users);
                //_canToggleUserStatus = await AuthService.HasPermissionAsync(state.User, FshActions.Update, FshResources.Users)
            }
            
            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => ApiClient.GetUserEndpointAsync(Id), Toast, Navigation)
                is { } result)
            {
                _model = result;

            }
            
            _loaded = true;
        }
        
        private void BackToUsers() =>  Navigation.NavigateTo("/identity/users");
        

        // private async Task UpdateUserAsync()
        private async Task UpdateUserDetails()
        {
            if (string.IsNullOrEmpty(Id)) return;
            
            if (LockoutEndDate != null && LockoutEndTime != null)
            {

                LockoutEndDate += LockoutEndTime;
                var timeSpan = LockoutEndDate - DateTime.Now;
                _model.LockoutEnd = DateTime.UtcNow.Add((TimeSpan)timeSpan);
            }

            var request = new UpdateUserCommand
                {
                Id = _model.Id.ToString(),
                
                FirstName = _model.FirstName,
                LastName = _model.LastName,
                UserName = _model.UserName,
                
                ImageUrl = _model.ImageUrl,
                
                Email = _model.Email,
                EmailConfirmed = _model.EmailConfirmed,
                PhoneNumber = _model.PhoneNumber,
                // PhoneNumberConfirmed = _model.PhoneNumberConfirmed,
           
                IsActive = _model.IsActive,
                IsOnline = _model.IsOnline,
                
                LockoutEnd = _model.LockoutEnd,
  
                CreatedOn = _model.CreatedOn,
                LastModifiedOn = _model.LastModifiedOn,
            };

            if (await ApiHelper.ExecuteCallGuardedAsync(() => ApiClient.UpdateUserEndpointAsync(Id, request ), Toast, 
                    null, "Your Profile has been updated"))
            {

                await OnInitializedAsync();
            }
            
            else { Toast.Add("Internal error.", Severity.Error); }
        }
        
        /// <summary>
        /// Temporarily lock a model account for 90 days
        /// </summary>
        /// <returns></returns>
        private async Task LockUserAccountAsync()
        {
            //var result = await ApiClient.LockUserAccountAsync(model);
            var result = await ApiClient.LockUserEndpointAsync(_model.Id.ToString());
            if (result)
            {
                Toast.Add("User account locked successfully.", Severity.Success);               
                return;
            }
            Toast.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Unlock a model account
        /// </summary>
        /// <returns></returns>
        private async Task UnlockUserAccountAsync()
        {
            var result = await ApiClient.UnLockUserEndpointAsync(_model.Id.ToString());
            if (result)
            {
                Toast.Add("User account unlocked successfully.", Severity.Success);
                this._model.LockoutEnd = null;
                return;
            }
            Toast.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }

        /// <summary>
        /// Remove an assigned role from model
        /// </summary>
        /// <param name="roleToDelete"></param>
        /// <returns></returns>
        
        private async Task RemoveRoleFromUserAsync(RoleSummaryDto roleToDelete)
        {
            if(_model.UserRoles == null) return;
            
            var request = new AssignUserRoleCommand
            {
                UserRoles =
                [
                    roleToDelete
                    // new UserRoleDetail
                    // {
                    //     RoleId = roleToDelete.Id,
                    //     RoleName = roleToDelete.Name,
                    //     Description = roleToDelete.Description,
                    //     Enabled = false
                    // }
                ]
            };

            if (await ApiHelper.ExecuteCallGuardedAsync(() => ApiClient.AssignRolesToUserEndpointAsync(Id, request),
                    Toast,
                    successMessage: "Role was successfully removed.") is { } result)
            {
                _model.UserRoles.Remove(roleToDelete);
            }
            
            if(!result)
            {
                Toast.Add($"Error while removing role.{result}", Severity.Error);
            }
            
        }
        private async Task AddRoleAsync()
        {
            
            var parameters = new DialogParameters
            {
                { "Owner", _model.Id.ToString() },
                { "ExistingRoles", _model.UserRoles },
                { "ApiClient", ApiClient }
            };
            var dialog = await Dialog.ShowAsync<AddRoleDialog>("Add New Role", parameters, new DialogOptions { MaxWidth = MaxWidth.Large, CloseButton = true });
            
            var result = await dialog.Result;         
            if (result != null && !result.Canceled && result.Data is RoleSummaryDto role)
            {
                _model.UserRoles?.Add(role);
                Toast.Add("Role successfully assigned.", Severity.Success);
            }
        }
        
        
        private async Task AddClaimAsync()
        {
            var parameters = new DialogParameters
            {
                { "Owner", _model.Id.ToString() },
                { "ExistingClaims", _model.UserClaims },
                { "ApiClient", ApiClient },
                { "ToRole", false}
            };
            
            var dialog = await Dialog.ShowAsync<AddClaimDialog>("Add Claim", parameters, new DialogOptions { MaxWidth = MaxWidth.ExtraLarge, CloseButton = true });
            
            var result = await dialog.Result;
            if (result is { Canceled: false, Data: ClaimViewModel claim })
            {
                _model.UserClaims?.Add(claim);
                Toast.Add($"Claim was added.", Severity.Success);
            }
        } 
        private async Task RemoveClaimAsync(ClaimViewModel claim)
        {
            if (_model.UserClaims != null && _model.UserClaims.Contains(claim))
            {
                var request = new RemoveClaimCommand
                {
                    Owner = _model.Id.ToString(),
                    ClaimToRemove = claim
                };
                
                if (await ApiHelper.ExecuteCallGuardedAsync(
                        () => ApiClient.RemoveClaimOfUserEndpointAsync(_model.Id.ToString(), request), 
                        Toast, successMessage: $"Claim {claim.Type}:{claim.Value} was removed.")
                    is { } result)
                {
                    _model.UserClaims.Remove(claim);
                }
                
                if(!result) Toast.Add($"Failed to delete claim {claim.Type}:{claim.Value}.", Severity.Error);
            }
        }
        private async Task<bool> UpdateClaimAsync(ClaimViewModel original, ClaimViewModel modified)
        {
            if (string.IsNullOrEmpty(_model.Id.ToString()))
            {
                Toast.Add($"UserId required.", Severity.Error);
                return false;
            }
            
            var request = new ChangeClaimCommand
            {
                Owner = _model.Id.ToString(),
                Original = original,
                Modified = modified
            };

            var result = await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.ChangeClaimOfUserEndpointAsync(_model.Id.ToString(), request),
                Toast, successMessage: $"Claim was updated.");
            
            if(!result) Toast.Add($"Failed to update claim .", Severity.Error);
            return result;
        }
    }
}
