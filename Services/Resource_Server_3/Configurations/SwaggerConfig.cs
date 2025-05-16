using Microsoft.OpenApi.Models;

namespace Resource_Server_3.Configurations;

public static class SwaggerConfig
{
    private const string SwaggerSpecUrl = "swagger/v1/swagger.json";
    //const string outputFolder = "{ProjectName}.Client/src/resources/api-clients/";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var identityHost = configuration["IdentityHost"] ;
        var identityServerUrl = string.IsNullOrEmpty(identityHost) ? "https://localhost:7000" : identityHost;
        
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "OAuth 2.0 Authorization Code Flow (with client secret)",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{identityServerUrl}/connect/authorize"),
                        TokenUrl = new Uri($"{identityServerUrl}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api1", "resource server scope" }
                        }
                    },
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, 
                            Id = "oauth2"
                        }
                    },
                    []
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerService(this WebApplication app)
    {
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("swagger-client");
                c.OAuthClientSecret("388D45FA-B36B-4988-BA59-B187D329C205");
                c.OAuthUsePkce();
                c.OAuthScopeSeparator(" ");
            });
        }
        
        return app;
    }
}
