using System.Net.Http.Headers;
using Blazor.BFF.OpenIddict.Client.Services;
using Client.Infrastructure.Services;
using Client.Infrastructure.Services.Resource_3;

namespace Blazor.BFF.OpenIddict.Client.Configurations;

public static class HttpClientRegistration
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services, string serverBaseAddress)
    {
        const string resoure3Address = "https://localhost:7203";

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
        
        services.AddHttpClient<IWeatherService, WeatherService>("resource3Client", client =>
        {
            client.BaseAddress = new Uri(resoure3Address);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<AuthorizedHandler>();

        services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
        services.AddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();
        
        return services;
    }
}
