using Identity.Core.Features.Claim;

namespace Identity.Core.Features.Role;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = [];
    
    public List<ClaimViewModel> Claims { get; set; } = [];
    
    public RoleDto()
    {

    }

    private RoleDto(string roleName)
    {
        Id = Guid.NewGuid();
        Name = roleName;
    }
    
    public RoleDto(Guid roleId, string roleName) : this(roleName)
    {
        Id = roleId;         
    }
}
