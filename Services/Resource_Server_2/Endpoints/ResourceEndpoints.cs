using Identity.Shared.Authorization;

namespace Resource_Server_2.Endpoints;

public static class ResourceEndpoints
{
    public static void MapResourceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/protected", (HttpContext context, ILogger<Program> logger) =>
        {
            var user = context.User.Identity?.Name ?? "Anonymous";
            return context.Response.WriteAsJsonAsync(new { user });
        })
            .WithOpenApi()
            .WithName("Protected endpoint")
            .RequireAuthorization();

        app.MapGet("/mustbeeditor", context =>
        {
            var user = context.User.Identity?.Name ?? "Anonymous";
            return context.Response.WriteAsJsonAsync(new { user });
        })
            .WithOpenApi()
            .WithName("Must be editor endpoint")
            .RequireAuthorization(AppScopes.AuthPolicy);

        app.MapGet("/unprotected", () => "Ladies and gentlemen, we got him")
            .WithOpenApi()
            .WithName("Unprotected endpoint");
    }
}