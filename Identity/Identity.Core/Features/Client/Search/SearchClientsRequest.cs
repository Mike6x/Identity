using BuildingBlocks.Paging;

namespace Identity.Core.Features.Client.Search;

public class SearchClientsRequest : PaginationFilter
{
    public string? RoleFilter { get; set; }
}