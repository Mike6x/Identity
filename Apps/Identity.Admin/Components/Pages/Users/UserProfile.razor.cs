using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.Common;
using Identity.Admin.Components.Dialogs;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Users;

public partial class UserProfile
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    
    [Inject]
    public required IDialogService Dialog { get; set; }
    
    [Inject]
    public required IApiClient UsersClient { get; set; } 
    
    [Parameter]
    public required  string Id { get; set; }
    
    private UserDto _model = new();
    
    private bool _loaded;
    private string Title => $"{_model.FirstName} {_model.LastName}'s Profile";
    private string Description => $"Id: {_model.Id}";
    private char FirstLetterOfName => _model.FirstName?.Length > 0 ? _model.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault() : 'U';
    
    private string Tenant { get; set; } = TenantConstants.Root.Id;

    private CustomValidation? _customValidation;
    
    private bool _canEditItems;
    
    private bool _canSearchItems;
    private string _searchString = string.Empty;
    
    private bool DeleteImageRequest { get; set; } = false;
    private Uri? ImageUrl { get; set; }

    private FileUploadCommand ImageUpload { get; set; } = new FileUploadCommand();
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
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetUserEndpointAsync(Id), Toast, Navigation)
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

        if (await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.SendVerificationEmailEndPointAsync(Id), Toast))
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
                () => UsersClient.ForgotPasswordEndpointAsync(Tenant, forgotPasswordRequest),
                Toast);

            Toast.Add("Reset email has been sent.", Severity.Success);
        }
        
        Toast.Add("Reset email is empty.", Severity.Error);

    }

    private async Task ToggleActiveStatusAsync()
    {
        var request = new ToggleUserStatusCommand { IsActive = !_model.IsActive, UserId = Id };
        var message = _model.IsActive ? "The Account have disabled" : "The Account have activated";
      
        if ( await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.ToggleUserStatusEndpointAsync(Id, request), Toast))
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
            ? await UsersClient.UnLockUserEndpointAsync(_model.Id.ToString())
            : await UsersClient.LockUserEndpointAsync(_model.Id.ToString());
        
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
                () => UsersClient.SetOnlineStatusEndpointAsync(Id, !_model.IsOnline), Toast))
        {
            Toast.Add(message, Severity.Success);
            _model.IsOnline  = !_model.IsOnline;
            await OnInitializedAsync(); 
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
           
            CreatedOn = _model.CreatedOn,
            LastModifiedOn = _model.LastModifiedOn,
            
        };
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.UpdateUserEndpointAsync(Id, request), Toast))
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
            byte[]? buffer = new byte[imageFile.Size];
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
    
}
