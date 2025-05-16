namespace Identity.Core.Settings;

public class UserConfig
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    
    public string Role { get; set; } = string.Empty;
}
