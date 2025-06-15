namespace Identity.Core.Features.User.ToggleUserStatus;
public class ToggleUserStatusCommand
{
    public bool IsActive { get; set; }
    public string? UserId { get; set; }
}
