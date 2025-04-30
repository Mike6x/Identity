using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Identity.API.EndPoints;

public static class StatusEndpoints
{
    public static IEndpointRouteBuilder MapStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetStatusEndpoint();

        return app;
    }
}


public static class GetStatusEndpoint
{
    public static RouteHandlerBuilder MapGetStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/status", StatusHandler )
            .WithName(nameof(GetStatusEndpoint))
            .WithSummary("Get status of application ")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Return status of Application");
    }

    private static async Task<Ok<StatusDto>> StatusHandler(HttpContext httpContext, ApplicationDbContext dbContext)
    {
        var status = await dbContext.Database.CanConnectAsync();


        return TypedResults.Ok(new StatusDto
        {
            Api = "Ok",
            Db = status ? "Ok" : "Error",
            TimeStamp = DateTime.UtcNow
        });
    }

    private class StatusDto
    {
        public string Api { get; set; }
        public string Db { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}