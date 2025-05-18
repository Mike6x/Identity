using Blazor.Wasm.Client.Configurations;
using Blazored.LocalStorage;
using Client.Infrastructure;

namespace Blazor.Wasm.Client;

public static class HostingExtensions
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        var settingAuthorityUrl = configuration["AuthorityUrl"];
        var authorityUrl = string.IsNullOrEmpty(settingAuthorityUrl)
                                    ? "https://localhost:7000"
                                    : settingAuthorityUrl;
        
        services.AddBlazoredLocalStorage();
        services.AddInfraServices();
        
        services.RegisterHttpClient(authorityUrl);
        
        services.AddCascadingAuthenticationState();
        // services.AddOpenIdDictConfig(authorityUrl);
        
        services.AddOpenIdDictCfg(configuration);
        
       return services;
    }
}