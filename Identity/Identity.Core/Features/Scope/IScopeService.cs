using BuildingBlocks.Paging;
using Identity.Core.Features.Scope.Create;
using Identity.Core.Features.Scope.Search;
using Identity.Core.Features.Scope.Update;
using Microsoft.AspNetCore.Http;

namespace Identity.Core.Features.Scope;

public interface IScopeService
{
    Task<IResult> CreateAsync(CreateScopeCommand request, CancellationToken cancellationToken);
    Task<ScopeDto> GetByIdAsync(string scopeId, CancellationToken cancellationToken);
    Task<List<ScopeSummaryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PagedList<ScopeSummaryDto>> SearchAsync(SearchScopesRequest request, CancellationToken cancellationToken);
    Task<IResult> DeleteAsync(string scopeId, CancellationToken cancellationToken);
    Task<IResult> UpdateAsync(UpdateScopeCommand request, CancellationToken cancellationToken);

}