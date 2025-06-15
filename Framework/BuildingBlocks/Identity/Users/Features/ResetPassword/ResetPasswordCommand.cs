namespace Identity.Core.Features.User.ResetPassword;
public class ResetPasswordCommand
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
