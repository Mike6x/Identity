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
    
    private readonly UpdateUserCommand _model = new();
    private string Title => $"{_model.FirstName} {_model.LastName}'s Profile";
    private string Description => $"Id: {_model.Id}";
    private bool IsOnline => _model.IsOnline?? false;
    
    private string Tenant { get; set; } = TenantConstants.Root.Id;

    private CustomValidation? _customValidation;
    
    private bool _canEditItems;
    
    private bool _canSearchItems;
    private string _searchString = string.Empty;
    
    private bool _loaded;
    private bool _isActive;
    private char _firstLetterOfName;
    private Uri? _imageUrl;
    

    private bool IsLocked { get; set; }
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
        }
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetUserEndpointAsync(Id), Toast, Navigation)
            is { } user)
        {
            _model.Id = user.Id.ToString();
            _model.FirstName = user.FirstName ?? string.Empty;
            _model.LastName = user.LastName ?? string.Empty;
            _model.UserName = user.UserName ?? string.Empty;
            _model.Email = user.Email ?? string.Empty;
            _model.PhoneNumber = user.PhoneNumber ?? string.Empty;
            _model.IsActive = _isActive = user.IsActive;

            _model.IsOnline = user.IsOnline;
            _model.EmailConfirmed = user.EmailConfirmed;

             _model.ImageUrl = user.ImageUrl;

            _model.CreatedBy = user.CreatedBy.ToString() ?? string.Empty;
            _model.CreatedOn = user.CreatedOn;
            _model.LastModifiedBy = user.LastModifiedBy.ToString() ?? string.Empty;
            _model.LastModifiedOn = user.LastModifiedOn ?? user.CreatedOn;

            if (_model.FirstName.Length > 0)
            {
                _firstLetterOfName = _model.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault();
            }

            if (user.LockoutEnd != null)
            {
                _model.LockoutEnd = (DateTime)user.LockoutEnd;

                LockoutEndDate = user.LockoutEnd.Value.ToLocalTime().Date;
                LockoutEndTime = user.LockoutEnd.Value.ToLocalTime().TimeOfDay;
                var now = DateTimeOffset.Now;
                IsLocked = user.LockoutEnd > now;
            }
            
            _imageUrl = user.ImageUrl;
            
        }


        _loaded = true;
    }

    private void BackToUsers() => Navigation.NavigateTo("/identity/users");
    
    private void BackToEmplyees() => Navigation.NavigateTo("/People/Employees");
    
   

    private async Task SendVerificationEmailAsync()
    {

        if (await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.SendVerificationEmailEndPointAsync(Id!), Toast))
        {
            Toast.Add("Verification email has been sent.", Severity.Success);
            _isActive = true;
            _model.IsActive = true;
            _model.EmailConfirmed = false;
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    private async Task SendRecoveryPasswordEmailAsync()
    {
        var forgotPasswordRequest = new ForgotPasswordCommand
        {
            Email = _model.Email!
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
            _model.IsActive = _isActive;
        }
        else { Toast.Add("Internal error.", Severity.Error); }

        // await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.ToggleUserStatusEndpointAsync(Id!, request), Toast)
        // Navigation.NavigateTo("/identity/users")
    }

    private async Task UnlockUserAsync()
    {
        _model.LockoutEnd = DateTime.UtcNow;
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.UpdateUserEndpointAsync(Id!, _model), Toast))
        {
            Toast.Add("User is unlocked.", Severity.Success);
            await OnInitializedAsync();
        }
        else { Toast.Add("Internal error.", Severity.Error); }
    }

    public async Task ClearOnlineStatus()
    {
        if(_model.IsOnline == true)
        {
            _model.IsOnline  = false;

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.UpdateUserEndpointAsync(Id!, _model), Toast))
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
            _model.LockoutEnd = DateTime.UtcNow.Add((TimeSpan)timeSpan);
        }

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.UpdateUserEndpointAsync(Id!, _model), Toast))
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
            _model.DeleteCurrentImage = true;
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
            _model.Image = new FileUploadCommand() { Name = fileName, Data = base64String, Extension = extension };

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
