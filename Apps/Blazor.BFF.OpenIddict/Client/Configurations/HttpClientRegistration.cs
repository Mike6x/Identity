using System.Net.Http.Headers;
using Blazor.BFF.OpenIddict.Client.Services;
using Client.Infrastructure.Services;

namespace Blazor.BFF.OpenIddict.Client.Configurations;

public static class HttpClientRegistration
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services, string serverBaseAddress)
    {

        services.AddHttpClient("default", client =>
        {
            client.BaseAddress = new Uri(serverBaseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddHttpClient("authorizedClient", client =>
        {
            client.BaseAddress = new Uri(serverBaseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<AuthorizedHandler>();
        
        services.AddHttpClient<IWeatherService, WeatherService>("Resource_3_Client", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7203/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<AuthorizedHandler>();

        services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
        services.AddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();
        
        return services;
    }
}
