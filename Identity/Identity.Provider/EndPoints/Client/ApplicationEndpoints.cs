using Identity.Core.Features.Client;
using Identity.Core.Features.Client.Search;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Provider.EndPoints.Client;

public static class ApplicationEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateApplicationEndpoint();
        app.MapGetApplicationEndpoint();
        app.MapGetApplicationByNameEndpoint();
        app.MapGetApplicationsEndpoint();
        app.MapSearchApplicationsEndpoint();
        
        app.MapUpdateApplicationEndpoint();
        app.MapDeleteApplicationEndpoint();
        
        app.MapCallBackApplicationEndpoint();

        return app;
    }
}

public static class CreateApplicationEndpoint
{
    public static RouteHandlerBuilder MapCreateApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", (ApplicationViewModel request, IApplicationService service, CancellationToken cancellationToken) 
                => service.CreateAsync(request, cancellationToken))
            .WithName(nameof(CreateApplicationEndpoint))
            .WithSummary("Create new application")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Create new Application.");
    }
}

public static class GetApplicationEndpoint
{
    public static RouteHandlerBuilder MapGetApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("{id}",async (string id, IApplicationService service, CancellationToken cancellationToken) 
                => await service.GetAsync(id, cancellationToken))
            .WithName(nameof(GetApplicationEndpoint))
            .WithSummary("Get Application details by Internal Id")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its ID.");
    }
}

public static class GetApplicationByNameEndpoint
{
    public static RouteHandlerBuilder MapGetApplicationByNameEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/clientId/{clientId}",async (string clientId, IApplicationService service, CancellationToken cancellationToken) 
                => await service.GetByNameAsync(clientId, cancellationToken))
            .WithName(nameof(GetApplicationByNameEndpoint))
            .WithSummary("Get Application details by Client Id")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its ID.");
    }
}

public static class GetApplicationsEndpoint
{
    public static RouteHandlerBuilder MapGetApplicationsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IApplicationService service, CancellationToken cancellationToken) 
                => await service.GetAllAsync(cancellationToken))
            .WithName(nameof(GetApplicationsEndpoint))
            .WithSummary("Get all clients ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return a list of all Applications.");
    }
}

public static class SearchApplicationsEndpoint
{
    public static RouteHandlerBuilder MapSearchApplicationsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", (SearchApplicationsRequest request, IApplicationService service, 
                CancellationToken cancellationToken)  
                => service.SearchAsync(request, cancellationToken))
            .WithName(nameof(SearchApplicationsEndpoint))
            .WithSummary("Search applications ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return a Paged list of Applications.");
    }
}

public static class DeleteApplicationEndpoint
{
    public static RouteHandlerBuilder MapDeleteApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{clientId}",  async (string clientId, IApplicationService service, CancellationToken cancellationToken) =>
            {
                await service.DeleteAsync(clientId, cancellationToken);
            })
            .WithName(nameof(DeleteApplicationEndpoint))
            .WithSummary("Remove application.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Remove Application.");
    }
}

public static class UpdateApplicationEndpoint
{
    public static RouteHandlerBuilder MapUpdateApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", (ApplicationViewModel request, IApplicationService service, CancellationToken cancellationToken) 
                => service.UpdateAsync(request, cancellationToken))
            .WithName(nameof(UpdateApplicationEndpoint))
            .WithSummary("Update application.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Update Application.");
    }
}

public static class CallBackApplicationEndpoint
{
    public static RouteHandlerBuilder MapCallBackApplicationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/callback", (
                    HttpContext httpContext, 
                    [FromServices] IHttpClientFactory httpClientFactory,
                    IApplicationService service) 
                => service.CallbackAsync(httpContext, httpClientFactory))
            .WithName(nameof(CallBackApplicationEndpoint))
            .WithSummary("Call back application.")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Call back Application.");
    }
}