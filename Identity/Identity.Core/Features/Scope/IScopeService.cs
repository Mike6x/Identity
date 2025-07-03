using BuildingBlocks.Paging;
using Identity.Core.Features.Scope.Create;
using Identity.Core.Features.Scope.Search;
using Identity.Core.Features.Scope.Update;
using Microsoft.AspNetCore.Http;

namespace Identity.Core.Features.Scope;

public interface IScopeService
{
    Task<IResult> CreateAsync(CreateScopeCommand request, CancellationToken cancellationToken);
    Task<ScopeDto> GetAsync(string scopeId, CancellationToken cancellationToken);
    Task<List<ScopeDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedList<ScopeDto>> SearchAsync(SearchScopesRequest request, CancellationToken cancellationToken);
    Task<IResult> DeleteAsync(string scopeId, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(UpdateScopeCommand request, CancellationToken cancellationToken);

}