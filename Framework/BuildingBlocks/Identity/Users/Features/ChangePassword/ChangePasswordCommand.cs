namespace Identity.Core.Features.User.ChangePassword;
public class ChangePasswordCommand
{
    public string Password { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
