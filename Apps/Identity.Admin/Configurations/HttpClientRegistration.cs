using System.Net.Http.Headers;

namespace Identity.Admin.Configurations;

public static class HttpClientRegistration
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TokenHandler>();
        
        var authorityUrl = configuration["OIDCSettings:Authority"];
        if (!string.IsNullOrEmpty(authorityUrl))
        {
            services.AddHttpClient("authorityClient", client => 
            { 
                client.BaseAddress = new Uri(authorityUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();
        }
        
        var gatewayUrl = configuration["OIDCSettings:ApiGateway"];
        if (!string.IsNullOrEmpty(gatewayUrl))
        {
            services.AddHttpClient("default", client => 
            { 
                client.BaseAddress = new Uri(gatewayUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();
        
            services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
        }
        
        return services;
    }
}
