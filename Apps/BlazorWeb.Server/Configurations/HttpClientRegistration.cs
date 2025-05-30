using System.Net.Http.Headers;

namespace BlazorWeb.Server.Configurations;

public static class HttpClientRegistration
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TokenHandler>();
        
        var resource1Url = configuration["ApiSettings:Resource_Server_1"];
        if (!string.IsNullOrEmpty(resource1Url))
        {
            services.AddHttpClient("resource1Client", client =>
            {
                client.BaseAddress = new Uri(resource1Url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();
        }
        
        var resource2Url = configuration["ApiSettings:Resource_Server_2"];
        if (!string.IsNullOrEmpty(resource2Url))
        {
            services.AddHttpClient("resource2Client", client =>
            {
                client.BaseAddress = new Uri(resource2Url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();
                
        }

        var resource3Url = configuration["ApiSettings:Resource_Server_3"];
        if (!string.IsNullOrEmpty(resource3Url))
        {
            services.AddHttpClient("resource3Client", client =>
            {
                client.BaseAddress = new Uri(resource3Url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();
        }

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
        
        // services.AddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();
        
        return services;
    }
}
