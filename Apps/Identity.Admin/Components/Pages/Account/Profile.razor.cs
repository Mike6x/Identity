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

    private UserDto _model = new();
    
    private char _firstLetterOfName = 'U';

    private AppValidation? _customValidation;
    

    private readonly UpdateUserCommand _request = new();

    private string? _imageUrl = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        if (AuthState == null) return;
        var principal = (await AuthState).User;
        {
            var subjectId = principal.FindFirst("sub")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(subjectId)) return;

            _model = await ApiClient.GetUserEndpointAsync(subjectId);
            
            _firstLetterOfName = _model.FirstName?.Length > 0 
                ? _firstLetterOfName.ToString().ToUpper(System.Globalization.CultureInfo.CurrentCulture).FirstOrDefault()
                : 'U';
        }
    }


    private async Task UpdateProfileAsync()
    {
        _request.Email = _model.Email;
        _request.FirstName = _model.FirstName;
        _request.LastName = _model.LastName;
        _request.PhoneNumber = _model.PhoneNumber;
        _request.UserName = _model.UserName;
            
        _request.Id = _model.Id.ToString();
        
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.UpdateCurrentUserEndpointAsync(_request), Toast, _customValidation))
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

            var fileName = $"{_model.Id}-{Guid.NewGuid():N}";
            fileName = fileName[..Math.Min(fileName.Length, 90)];
            var imageFile = await file.RequestImageFileAsync(AppConstants.StandardImageFormat, AppConstants.MaxImageWidth, AppConstants.MaxImageHeight);
            var buffer = new byte[imageFile.Size];
            _ = await imageFile.OpenReadStream(AppConstants.MaxAllowedSize).ReadAsync(buffer);
            var base64String = $"data:{AppConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            _request.Image = new FileUploadCommand { Name = fileName, Data = base64String, Extension = extension };

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
            _request.DeleteCurrentImage = true;
            await UpdateProfileAsync();
        }
    }
    
    private void ViewProfile() =>
        Navigation.NavigateTo($"/identity/users/{_model.Id}/details");
}
