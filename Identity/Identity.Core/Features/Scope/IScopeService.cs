using BuildingBlocks.Paging;
using Identity.Core.Features.Scope.Search;
using Microsoft.AspNetCore.Http;

namespace Identity.Core.Features.Scope;

public interface IScopeService
{
    Task<ScopeViewModel> CreateAsync(ScopeViewModel scopeDescriptor, CancellationToken cancellationToken);
    Task<ScopeViewModel> GetAsync(string scopeId, CancellationToken cancellationToken);
    Task<List<ScopeViewModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedList<ScopeViewModel>> SearchAsync(SearchScopesRequest request, CancellationToken cancellationToken);
    Task<IResult> DeleteAsync(string scopeId, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(ScopeViewModel descriptor, CancellationToken cancellationToken);

}