using BuildingBlocks.Paging;

namespace Identity.Core.Features.Client.Search;

public class SearchApplicationsRequest : PaginationFilter
{
    public string? RoleFilter { get; set; }
}