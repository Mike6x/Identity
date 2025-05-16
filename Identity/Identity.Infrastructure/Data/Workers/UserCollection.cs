using System.Security.Claims;

namespace Identity.Infrastructure.Data.Workers;

public class UserCollection
{
    private readonly List<User> _users = [];
   
    public IEnumerable<User> GetAllUsers() => _users;

    public UserCollection()
    {
        _users.Add(new User("test_user_1@pixel.com", "tesT-useR-secreT-1"));
        _users.Add(new User("test_user_2@pixel.com", "tesT-useR-secreT-2"));

    }
   
}

public class User(string email, string password)
{
    public string Email { get; set; } = email;

    public string Password { get; set; } = password;

    public List<string> Roles { get; private set; } = [];

    public List<Claim> Claims { get; private set; } = [];
}

