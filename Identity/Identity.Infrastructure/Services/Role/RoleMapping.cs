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
        };
    }
    public static RoleDto ToDetailDto(this AppRole source, List<ClaimViewModel> claims)
    {
        return  new RoleDto
        {
            Id = source.Id, 
            Name = source.Name?? string.Empty, 
            Description = source.Description,
            Claims = claims
        };
    }

    public static AppRole ToModel(this RoleDto source)
    {
        return new AppRole( source.Name, source.Description);
    }
}