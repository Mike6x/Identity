using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.Common;
using Identity.Admin.Components.Dialogs;
using Identity.Admin.Components.OpenIddict;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Users;

public partial class UserDetails
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
    public required  string Id { get; set; }
    
    private UserDto _model = new();
    
    private bool _loaded;
    private string Title => $"{_model.FirstName} {_model.LastName}'s Profile";
    private string Description => $"Id: {_model.Id}";
    private char FirstLetterOfName => _model.FirstName?.Length > 0 ? _model.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault() : 'U';
    
    private string Tenant { get; set; } = TenantConstants.Root.Id;

    private CustomValidation? _customValidation;
    
    private string _currentUserId = string.Empty;
    private bool _canEditItems;
    
    private bool _canSearchItems;
    private string _searchString = string.Empty;
    
    private bool DeleteImageRequest { get; set; }
    private Uri? ImageUrl { get; set; }

    private FileUploadCommand ImageUpload { get; set; } = new();
    private DateTime? LockoutEndDate { get; set; }
    private TimeSpan? LockoutEndTime { get; set; }
    
    private string Password { get; set; } = string.Empty;
    private string ConfirmPassword { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (AuthState == null || string.IsNullOrEmpty(Id)) return;
            
        var state = await AuthState;
   
        if (AuthService != null)
        {
            _canEditItems = await AuthService.HasPermissionAsync(state.User, AppActions.Update, AppResources.Users);
            _canSearchItems = await AuthService.HasPermissionAsync(state.User, AppActions.Search, AppResources.Users);
        }
        
        _currentUserId = state.User.GetUserId() ?? string.Empty;
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetUserEndpointAsync(Id), Toast, Navigation)
            is { } user)
        {
            
            _model = user;

            if (user.LockoutEnd != null)
            {
                LockoutEndDate = user.LockoutEnd.Value.ToLocalTime().Date;
                LockoutEndTime = user.LockoutEnd.Value.ToLocalTime().TimeOfDay;
            }
            
            ImageUrl = user.ImageUrl;
            
        }


        _loaded = true;
    }

    private void BackToUsers() => Navigation.NavigateTo("/identity/users");
    
    private void BackToEmployees() => Navigation.NavigateTo("/People/Employees");
    

    private async Task SendVerificationEmailAsync()
    {

        if (await ApiHelper.ExecuteCallGuardedAsync(() => ApiClient.SendVerificationEmailEndPointAsync(Id), Toast))
        {
            Toast.Add("Verification email has been sent.", Severity.Success);
  
            _model.IsActive = true;
            _model.EmailConfirmed = false;
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    private async Task SendRecoveryPasswordEmailAsync()
    {
        if (!string.IsNullOrEmpty(_model.Email))
        {
            var forgotPasswordRequest = new ForgotPasswordCommand
            {
                Email = _model.Email
            };

            await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.ForgotPasswordEndpointAsync(Tenant, forgotPasswordRequest),
                Toast);

            Toast.Add("Reset email has been sent.", Severity.Success);
        }
        
        Toast.Add("Reset email is empty.", Severity.Error);

    }

    private async Task ToggleActiveStatusAsync()
    {
        var request = new ToggleUserStatusCommand { IsActive = !_model.IsActive, UserId = Id };
        var message = _model.IsActive ? "The Account have disabled" : "The Account have activated";
      
        if ( await ApiHelper.ExecuteCallGuardedAsync(() => ApiClient.ToggleUserStatusEndpointAsync(Id, request), Toast))
        {
            Toast.Add(message, Severity.Success);
            _model.IsActive = !_model.IsActive;
        }
        else { Toast.Add("Internal error.", Severity.Error); }
        
    }
    
    private async Task ToggleLockedStatusAsync()
    {
        var message = _model.IsLocked ? "The Account have unlocked" : "User account locked for 30 days .";
        
        var result = _model.IsLocked 
            ? await ApiClient.UnLockUserEndpointAsync(_model.Id.ToString())
            : await ApiClient.LockUserEndpointAsync(_model.Id.ToString());
        
        if (result)
        {
            Toast.Add(message, Severity.Success);               
            await OnInitializedAsync();
        }
        else
        {
            Toast.Add("Internal error.", Severity.Error);
        }
    }
    
    private async Task ToggleOnlineStatus()
    {
        var message = _model.IsOnline ? "The User is offline now" : "The user is online now";

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.SetOnlineStatusEndpointAsync(Id, !_model.IsOnline), Toast))
        {
            Toast.Add(message, Severity.Success);
            _model.IsOnline  = !_model.IsOnline;
        }
        else { Toast.Add("Internal error.", Severity.Error); }
        
    }
    

    private async Task UpdateUserAsync()
    {
        var request = new UpdateUserCommand
        {
            Id = _model.Id.ToString(),

            FirstName = _model.FirstName,
            LastName = _model.LastName,
            UserName = _model.UserName,
            
            ImageUrl = ImageUrl,
            DeleteCurrentImage = DeleteImageRequest,
            Image = ImageUpload,
            
            Email = _model.Email,
            EmailConfirmed = _model.EmailConfirmed,
            PhoneNumber = _model.PhoneNumber,
            
            IsActive = _model.IsActive,
            IsOnline = _model.IsOnline,
            
            LockoutEnd = ConvertToUtcDateTime(LockoutEndDate,LockoutEndTime),
            
            Password = Password,
            ConfirmPassword = ConfirmPassword,
           
            CreatedOn = _model.CreatedOn,
            LastModifiedOn = DateTime.UtcNow,
            
            LastModifiedBy = _currentUserId
        };
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.UpdateUserEndpointAsync(Id, request), Toast))
        {
            Toast.Add("Your Profile has been updated.", Severity.Success);
            await OnInitializedAsync();
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    private static DateTime? ConvertToUtcDateTime(DateTime? localDate, TimeSpan? localTime)
    {
        if (localDate == null && localTime == null)
            return null;

        var date = (localDate ?? DateTime.Now.Date).Date;
        var time = localTime ?? DateTime.Now.TimeOfDay;

        var localDateTime = date.Add(time);
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime);
    }

    public async Task RemoveImageAsync()
    {
        const string deleteContent = "You're sure you want to delete your Profile Image?";
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), deleteContent }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>("Delete", parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            DeleteImageRequest = true;
            await UpdateUserAsync();
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var file = e.File;
        {
            var extension = Path.GetExtension(file.Name);
            if (!AppConstants.SupportedImageFormats.Contains(extension.ToLower(System.Globalization.CultureInfo.CurrentCulture)))
            {
                Toast.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            var fileName = $"{Id}-{Guid.NewGuid():N}";
            fileName = fileName[..Math.Min(fileName.Length, 90)];
            var imageFile = await file.RequestImageFileAsync(AppConstants.StandardImageFormat, AppConstants.MaxImageWidth, AppConstants.MaxImageHeight);
            byte[] buffer = new byte[imageFile.Size];
            _ = await imageFile.OpenReadStream(AppConstants.MaxAllowedSize).ReadAsync(buffer);
            var base64String = $"data:{AppConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            
            ImageUpload = new FileUploadCommand() { Name = fileName, Data = base64String, Extension = extension };

            await UpdateUserAsync();
        }
    }

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }
    
    
    private async Task RemoveRoleFromUserAsync(RoleSummaryDto roleToDelete)
    {
        if(_model.UserRoles == null) return;
            
        var request = new AssignUserRoleCommand
        {
            UserRoles = [roleToDelete]
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
