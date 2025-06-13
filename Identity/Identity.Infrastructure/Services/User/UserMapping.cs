using Identity.Core.Entities;
using Identity.Core.Features.User.Dtos;

namespace Identity.Infrastructure.Services.User;

public static class UserMapping
{
    public static UserDto ToDto(AppUser user, IList<string> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,

        };
    }
    
}