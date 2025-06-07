using Identity.Core.Features.Claim;

namespace Identity.Core.Features.Role.CreateOrUpdateRole;

public class CreateRoleCommand
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public List<ClaimViewModel> Claims { get; set; } = [];
}