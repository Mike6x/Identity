using Client.Infrastructure.Api;
using Identity.Admin.Components.Dialogs;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Account;

public partial class Profile
{
    [CascadingParameter]
    protected Task<AuthenticationState>? AuthState { get; set; }
    
    [Inject]
    protected IAuthenticationService? AuthService { get; set; }
    
    [Inject]
    public required IApiClient ApiClient { get; set; } 

    private readonly UpdateUserCommand _model = new();

    private string? _imageUrl = string.Empty;
    
    private Guid _userId = Guid.Empty;
    
    private char _firstLetterOfName;

    private AppValidation? _customValidation;

    protected override async Task OnInitializedAsync()
    {
        
        if (AuthState != null 
            && (await AuthState).User is { Identity: not null } user 
            && !string.IsNullOrEmpty(user.Identity.Name))
        {
            var userProfile = await ApiClient.GetUserByEmailEndpointAsync(user.Identity.Name);
            _userId  = userProfile.Id;
            
            _model.Email = userProfile.Email;
            _model.FirstName = userProfile.FirstName;
            _model.LastName = userProfile.LastName;
            _model.PhoneNumber = userProfile.PhoneNumber;
            _model.UserName = userProfile.UserName;
            
            _model.Id = _userId.ToString();
        }
        
        if (_model.FirstName?.Length > 0)
        {
            _firstLetterOfName = _model.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault();
        }
    }


    private async Task UpdateProfileAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.UpdateCurrentUserEndpointAsync(_model), Toast, _customValidation))
        {
            Toast.Add("Your Profile has been updated. Please Login again to get update.", Severity.Success);
            // await AuthService.ReLoginAsync(Navigation.Uri);
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

            var fileName = $"{_userId}-{Guid.NewGuid():N}";
            fileName = fileName[..Math.Min(fileName.Length, 90)];
            var imageFile = await file.RequestImageFileAsync(AppConstants.StandardImageFormat, AppConstants.MaxImageWidth, AppConstants.MaxImageHeight);
            var buffer = new byte[imageFile.Size];
            _ = await imageFile.OpenReadStream(AppConstants.MaxAllowedSize).ReadAsync(buffer);
            var base64String = $"data:{AppConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            _model.Image = new FileUploadCommand { Name = fileName, Data = base64String, Extension = extension };

            await UpdateProfileAsync();
        }
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
            _model.DeleteCurrentImage = true;
            await UpdateProfileAsync();
        }
    }
    
    private void ViewProfile() =>
        Navigation.NavigateTo($"/identity/users/{_userId}/profile");
}
