using BuildingBlocks.Paging;
using Identity.Core.Features.Client.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Core.Features.Client;

public interface IApplicationService
{
    Task<ApplicationViewModel?> CreateAsync(ApplicationViewModel applicationDescriptor,
        CancellationToken cancellationToken);

    Task<ApplicationViewModel> GetAsync(string id, CancellationToken cancellationToken);
    
    Task<ApplicationViewModel> GetByNameAsync(string clientId, CancellationToken cancellationToken);
    
    Task<List<ApplicationViewModel>> GetAllAsync(CancellationToken cancellationToken);

    Task<PagedList<ApplicationViewModel>> SearchAsync(SearchApplicationsRequest request,
        CancellationToken cancellationToken);

    Task<IResult> DeleteAsync(string clientId, CancellationToken cancellationToken);

    Task<IResult> UpdateAsync(ApplicationViewModel applicationDescriptor, CancellationToken cancellationToken);

    Task<IResult> CallbackAsync(HttpContext httpContext, [FromServices] IHttpClientFactory httpClientFactory);
}