using BuildingBlocks.Identity.Users.Dtos;
using Identity.Core.Entities;
using Identity.Core.Features.Claim;
using Identity.Core.Features.Role;

namespace Identity.Infrastructure.Services.Role;

public static class RoleMapping
{
    public static RoleDto ToDto(this AppRole source)
    {
        return  new RoleDto
        {
            Id = source.Id, 
            Name = source.Name?? string.Empty, 
            Description = source.Description,
            Claims = [],
            Permissions = []
        };
    }
    public static RoleSummaryDto ToSummaryDto(this AppRole source)
    {
        return  new RoleSummaryDto
        {
            Id = source.Id, 
            Name = source.Name?? string.Empty, 
            Description = source.Description,
            Enabled = false
            
        };
    }

    public static AppRole ToModel(this RoleDto source)
    {
        return new AppRole( source.Name, source.Description);
    }
}