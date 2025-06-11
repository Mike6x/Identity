using Client.Infrastructure.Api;
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
    protected Task<AuthenticationState> AuthState { get; set; } = null!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = null!;
    [Inject]
    protected IApiClient UsersClient { get; set; } = null!;

    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public string? Description { get; set; }

    private string Tenant { get; set; } = TenantConstants.Root.Id;
    private readonly UpdateUserCommand _profileModel = new();

    private CustomValidation? _customValidation;

    private bool _loaded;
    private bool _isActive;
    private char _firstLetterOfName;
    private Uri? _imageUrl;

    private bool IsOnline => _profileModel.IsOnline?? false;

    private bool IsLocked { get; set; }
    private DateTime? LockoutEndDate { get; set; }
    private TimeSpan? LockoutEndTime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetUserEndpointAsync(Id!), Toast, Navigation)
            is UserDetail user)
        {
            _profileModel.Id = user.Id.ToString();
            _profileModel.FirstName = user.FirstName ?? string.Empty;
            _profileModel.LastName = user.LastName ?? string.Empty;
            _profileModel.UserName = user.UserName ?? string.Empty;
            _profileModel.Email = user.Email ?? string.Empty;
            _profileModel.PhoneNumber = user.PhoneNumber ?? string.Empty;
            _profileModel.IsActive = _isActive = user.IsActive;

            _profileModel.IsOnline = user.IsOnline;
            _profileModel.EmailConfirmed = user.EmailConfirmed;

             _profileModel.ImageUrl = user.ImageUrl;

            _profileModel.CreatedBy = user.CreatedBy.ToString() ?? string.Empty;
            _profileModel.CreatedOn = user.CreatedOn;
            _profileModel.LastModifiedBy = user.LastModifiedBy.ToString() ?? string.Empty;
            _profileModel.LastModifiedOn = user.LastModifiedOn ?? user.CreatedOn;

            if (_profileModel.FirstName.Length > 0)
            {
                _firstLetterOfName = _profileModel.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault();
            }

            if (user.LockoutEnd != null)
            {
                _profileModel.LockoutEnd = (DateTime)user.LockoutEnd;

                LockoutEndDate = user.LockoutEnd.Value.ToLocalTime().Date;
                LockoutEndTime = user.LockoutEnd.Value.ToLocalTime().TimeOfDay;
                var now = DateTimeOffset.Now;
                IsLocked = user.LockoutEnd > now;
            }


            _imageUrl = user.ImageUrl;
            Title = $"{_profileModel.FirstName} {_profileModel.LastName}'s Profile";
            Description = $"Id: {_profileModel.Id}";

        }

        var state = await AuthState;
        //_canToggleUserStatus = await AuthService.HasPermissionAsync(state.User, FshActions.Update, FshResources.Users)
        _loaded = true;
    }

    private void BackToUsers()
    {
        Navigation.NavigateTo("/identity/users");
    }

    private void BackToEmplyees()
    {
        Navigation.NavigateTo("/People/Employees");
    }
   

    private async Task SendVerificationEmailAsync()
    {

        if (await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.SendVerificationEmailEndPointAsync(Id!), Toast))
        {
            Toast.Add("Verification email has been sent.", Severity.Success);
            _isActive = true;
            _profileModel.IsActive = true;
            _profileModel.EmailConfirmed = false;
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    private async Task SendRecoveryPasswordEmailAsync()
    {
        var forgotPasswordRequest = new ForgotPasswordCommand
        {
            Email = _profileModel.Email!
        };

        await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.ForgotPasswordEndpointAsync(Tenant, forgotPasswordRequest),
            Toast);

        Toast.Add("Reset email has been sent.", Severity.Success);
    }

    private async Task ToggleUserStatusAsync()
    {
        var request = new ToggleUserStatusCommand { IsActive = !_isActive, UserId = Id };
        
        if ( await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.ToggleUserStatusEndpointAsync(Id!, request), Toast))
        {
            string message = _isActive ? "The Account have disabled" : "The Account have activated";
            Toast.Add(message, Severity.Success);
            _isActive = !_isActive!;
            _profileModel.IsActive = _isActive;
        }
        else { Toast.Add("Internal error.", Severity.Error); }

        // await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.ToggleUserStatusEndpointAsync(Id!, request), Toast)
        // Navigation.NavigateTo("/identity/users")
    }

    private async Task UnlockUserAsync()
    {
        _profileModel.LockoutEnd = DateTime.UtcNow;
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.UpdateUserEndpointAsync(Id!, _profileModel), Toast))
        {
            Toast.Add("User is unlocked.", Severity.Success);
            await OnInitializedAsync();
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    public async Task ClearOnlineStatus()
    {
        if(_profileModel.IsOnline == true)
        {
            _profileModel.IsOnline  = false;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.UpdateUserEndpointAsync(Id!, _profileModel), Toast))
            {
                Toast.Add("User status is offline now.", Severity.Success);
                await OnInitializedAsync();
            }
        }
    }

    private async Task UpdateUserAsync()
    {
        if (LockoutEndDate != null && LockoutEndTime != null)
        {

            LockoutEndDate += LockoutEndTime;
            var timeSpan = LockoutEndDate - DateTime.Now;
            _profileModel.LockoutEnd = DateTime.UtcNow.Add((TimeSpan)timeSpan);
        }

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.UpdateUserEndpointAsync(Id!, _profileModel), Toast))
        {
            Toast.Add("Your Profile has been updated.", Severity.Success);
            await OnInitializedAsync();
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    public async Task RemoveImageAsync()
    {
        string deleteContent = "You're sure you want to delete your Profile Image?";
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), deleteContent }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>("Delete", parameters, options);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            _profileModel.DeleteCurrentImage = true;
            await UpdateUserAsync();
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is not null)
        {
            string? extension = Path.GetExtension(file.Name);
            if (!AppConstants.SupportedImageFormats.Contains(extension.ToLower(System.Globalization.CultureInfo.CurrentCulture)))
            {
                Toast.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            string? fileName = $"{Id}-{Guid.NewGuid():N}";
            fileName = fileName[..Math.Min(fileName.Length, 90)];
            var imageFile = await file.RequestImageFileAsync(AppConstants.StandardImageFormat, AppConstants.MaxImageWidth, AppConstants.MaxImageHeight);
            byte[]? buffer = new byte[imageFile.Size];
            _ = await imageFile.OpenReadStream(AppConstants.MaxAllowedSize).ReadAsync(buffer);
            string? base64String = $"data:{AppConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            _profileModel.Image = new FileUploadCommand() { Name = fileName, Data = base64String, Extension = extension };

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
