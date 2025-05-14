using Blazored.LocalStorage;
using Identity.UI.Client.Configurations;
using Identity.UI.Infrastructure;

namespace Identity.UI.Client;

public static class Extensions
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        var settingAuthorityUrl = configuration["AuthorityUrl"];
        var authorityUrl = string.IsNullOrEmpty(settingAuthorityUrl)
                                    ? "https://localhost:7000"
                                    : settingAuthorityUrl;
        
        services.AddBlazoredLocalStorage();
        services.RegisterServices();
        
        services.RegisterHttpClient(authorityUrl);
        services.AddOpenIdDictConfig(authorityUrl);
        
       return services;
    }
}