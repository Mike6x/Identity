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
    // [Inject]
    // protected IAuthenticationService? AuthService { get; set; }
    [Inject]
    protected IApiClient PersonalClient { get; set; } = null!;

    private readonly UpdateUserCommand _profileModel = new();

    private string? _imageUrl = string.Empty;
    private string? _userId = string.Empty;
    
    private char _firstLetterOfName;

    private FshValidation? _customValidation;

    protected override async Task OnInitializedAsync()
    {
        if (AuthState != null && (await AuthState).User is { } user)
        {
            var userProfile = await PersonalClient.GetUserByNameEndpointAsync(user.Identity!.Name);
            _profileModel.Id = userProfile.Id.ToString();
            _profileModel.Email = userProfile.Email;
            _profileModel.FirstName = userProfile.FirstName;
            _profileModel.LastName = userProfile.LastName;
            _profileModel.PhoneNumber = userProfile.PhoneNumber;
            _profileModel.UserName = userProfile.UserName;
            
            if (_userId is not null) _profileModel.Id = _userId;
        }

        if (_profileModel.FirstName?.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault();
        }
    }

    // protected override async Task OnInitializedAsync()
    // {
    //     if ((await AuthState).User is { } user)
    //     {
    //         _userId = user.GetUserId();
    //         _profileModel.Email = user.GetEmail() ?? string.Empty;
    //         _profileModel.FirstName = user.GetFirstName() ?? string.Empty;
    //         _profileModel.LastName = user.GetSurname() ?? string.Empty;
    //         _profileModel.PhoneNumber = user.GetPhoneNumber();
    //         _profileModel.ImageUrl = user.GetImageUrl();
    //         if (user.GetImageUrl() != null)
    //         {
    //             _imageUrl = user.GetImageUrl()!.ToString();
    //         }
    //         if (_userId is not null) _profileModel.Id = _userId;
    //     }
    //
    //     if (_profileModel.FirstName?.Length > 0)
    //     {
    //         _firstLetterOfName = _profileModel.FirstName.ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault();
    //     }
    // }
    private async Task UpdateProfileAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => PersonalClient.UpdateCurrentUserEndpointAsync(_profileModel), Toast, _customValidation))
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
            _profileModel.Image = new FileUploadCommand { Name = fileName, Data = base64String, Extension = extension };

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
            _profileModel.DeleteCurrentImage = true;
            await UpdateProfileAsync();
        }
    }
}
