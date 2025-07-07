using BuildingBlocks.Identity.Users.Dtos;
using Identity.Core.Entities;

namespace Identity.Infrastructure.Services.User;

public static class UserMapping
{
    public static UserDto ToDto(this AppUser user)
    {
        return new UserDto
        {
            Id = user.Id,
      
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            
            ImageUrl = user.ImageUrl,
            
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            
            IsActive = user.IsActive,
            IsOnline = user.IsOnline?? false,
            
            LockoutEnd = user.LockoutEnd,
            LastLoginOn = user.LastLoginOn,
            
            CreatedBy  = user.CreatedBy,
            CreatedOn = user.CreatedOn,
            LastModifiedBy = user.LastModifiedBy,
            LastModifiedOn = user.LastModifiedOn,
            
            UserRoles = [],
            UserClaims = []
        };
    }
    
    public static UserSummaryDto ToSummaryDto(this AppUser user)
    {
        return new UserSummaryDto
        {
            Id = user.Id,
      
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            
            IsActive = user.IsActive,
            IsOnline = user.IsOnline?? false,
            
            LockoutEnd = user.LockoutEnd,
            LastLoginOn = user.LastLoginOn,
            CreatedOn = user.CreatedOn,
        };
    }
    
    public static UserOnlineDto ToOnlineDto(this AppUser user)
    {
        return new UserOnlineDto
        {
            Id = user.Id,
      
            UserName = user.UserName,
            
            Email = user.Email,
            
            IsActive = user.IsActive,
            IsOnline = user.IsOnline?? false,
            
            LockoutEnd = user.LockoutEnd,
            LastLoginOn = user.LastLoginOn,
        };
    }
    
}