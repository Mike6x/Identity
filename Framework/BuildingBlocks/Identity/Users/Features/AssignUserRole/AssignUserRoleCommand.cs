using BuildingBlocks.Identity.Users.Dtos;

namespace Identity.Core.Features.User.AssignUserRole;

public class AssignUserRoleCommand
{
    public List<RoleSummaryDto> UserRoles { get; set; } = [];
}
