using BuildingBlocks.Paging;

namespace Identity.Core.Features.Scope.Search;

public class SearchScopesRequest : PaginationFilter
{
    public string ScopesFilter { get; set; } = string.Empty;
}