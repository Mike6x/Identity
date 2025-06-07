using BuildingBlocks.Paging;

namespace Identity.Core.Features.Role.SearchRoles;

public class SearchRolesRequest : PaginationFilter
{
    public string? RoleFilter { get; set; }
}