using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Resource_Server_1.Endpoints;

public static class ResourceEndpoints
{
    public static void MapResourceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/logout", (HttpContext context) =>
            {
                context.SignOutAsync("Cookies");
                context.SignOutAsync("OpenIddict.Server.AspNetCore");
            })
            .WithOpenApi()
            .WithName("Logout endpoint");
        
        app.MapGet("/currentUser", (HttpContext context, ILogger<Program> logger) =>
            {
                var user = context.User.Identity?.Name ?? "Anonymous";
                return $"user: {user}";
            })
            .WithOpenApi()
            .WithName("logon_user endpoint")
            .RequireAuthorization();
        
        app.MapGet("/unprotected", () => "Ladies and gentlemen, we got him")
            .WithOpenApi()
            .WithName("Unprotected endpoint");
        
        app.MapGet("/protected", (HttpContext context, ILogger<Program> logger) =>
            {
                var user = context.User.Identity?.Name ?? "Anonymous";
                return context.Response.WriteAsJsonAsync(new { user });
            })
            .WithOpenApi()
            .WithName("Protected endpoint")
            .RequireAuthorization();

        app.MapGet("/mustbeEditor", context =>
            {
                var user = context.User.Identity?.Name ?? "Anonymous";
                return context.Response.WriteAsJsonAsync(new { user });
            })
            .WithOpenApi()
            .WithName("Must be editor endpoint")
            .RequireAuthorization(AppScopes.AuthPolicy);
        
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        app.MapGet("/protectWeather", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)], "You should go out"
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi()
            .RequireAuthorization();
        
        app.MapGet("/unprotectWeather", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)], "You should stay home"
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherInfo")
           
            .WithOpenApi();
    }

    private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? Recommendation)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}