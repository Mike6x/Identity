using BuildingBlocks.Paging;
using Identity.Core.Features.Client.Create;
using Identity.Core.Features.Client.Search;
using Identity.Core.Features.Client.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Core.Features.Client;

public interface IClientService
{
    Task<IResult> CreateAsync(CreateClientCommand request, CancellationToken cancellationToken);

    Task<ApplicationDto> GetByIdAsync(string id, CancellationToken cancellationToken);
    
    Task<ApplicationDto> GetByNameAsync(string clientId, CancellationToken cancellationToken);
    
    Task<List<ApplicationSummaryDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<PagedList<ApplicationSummaryDto>> SearchAsync(SearchClientsRequest request,
        CancellationToken cancellationToken);

    Task<IResult> DeleteAsync(string id, CancellationToken cancellationToken);

    Task<IResult> UpdateAsync(UpdateClientCommand request, CancellationToken cancellationToken);

    Task<IResult> CallbackAsync(HttpContext httpContext, [FromServices] IHttpClientFactory httpClientFactory);
}