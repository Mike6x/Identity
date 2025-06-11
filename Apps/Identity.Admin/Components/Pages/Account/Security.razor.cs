using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Account;

public partial class Security
{
    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;
    
    [Inject]
    public IApiClient PersonalClient { get; set; } = default!;

    private readonly ChangePasswordCommand _passwordModel = new();

    private FshValidation? _customValidation;

    private async Task ChangePasswordAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => PersonalClient.ChangePasswordEndpointAsync(_passwordModel),
            Toast,
            _customValidation,
            "Password Changed!"))
        {
            _passwordModel.Password = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _passwordModel.ConfirmNewPassword = string.Empty;
            
            Toast.Add("Please Login again to Continue.", Severity.Success);
            // await AuthService.ReLoginAsync(Navigation.Uri);
        }
    }

    private bool _currentPasswordVisibility;
    private InputType _currentPasswordInput = InputType.Password;
    private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _newPasswordVisibility;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (_newPasswordVisibility)
            {
                _newPasswordVisibility = false;
                _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _newPasswordInput = InputType.Password;
            }
            else
            {
                _newPasswordVisibility = true;
                _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                _newPasswordInput = InputType.Text;
            }
        }
        else
        {
            if (_currentPasswordVisibility)
            {
                _currentPasswordVisibility = false;
                _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _currentPasswordInput = InputType.Password;
            }
            else
            {
                _currentPasswordVisibility = true;
                _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                _currentPasswordInput = InputType.Text;
            }
        }
    }
}
