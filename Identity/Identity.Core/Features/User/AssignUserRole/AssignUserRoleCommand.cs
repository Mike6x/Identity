using Identity.Core.Features.User.Dtos;

namespace Identity.Core.Features.User.AssignUserRole;

public class AssignUserRoleCommand
{
    public List<UserRoleDetail> UserRoles { get; set; } = [];
}
