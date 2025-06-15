using BuildingBlocks.Paging;

namespace Identity.Core.Features.User.SearchUsers;

public class SearchUsersRequest : PaginationFilter
{
    public bool? IsActive { get; set; }
}
