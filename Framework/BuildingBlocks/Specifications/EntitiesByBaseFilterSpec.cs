using Ardalis.Specification;
using BuildingBlocks.Paging;

namespace BuildingBlocks.Specifications;

public class EntitiesByBaseFilterSpec<T, TResult> : Specification<T, TResult> where T : class
{
    public EntitiesByBaseFilterSpec(BaseFilter filter) =>
        Query.SearchBy(filter);
}

public class EntitiesByBaseFilterSpec<T> : Specification<T> where T : class
{
    public EntitiesByBaseFilterSpec(BaseFilter filter) =>
        Query.SearchBy(filter);
}
