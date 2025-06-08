using System.Text.Json.Serialization;

namespace Identity.Core.Features.User.CreateUser;

// public class CreateUserCommand : IRequest<CreateUserResponse>
public class CreateUserCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    [JsonIgnore]
    public string? Origin { get; set; }
}
