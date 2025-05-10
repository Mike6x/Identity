using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using NSwag;
using NSwag.AspNetCore;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation.Processors.Security;

namespace Resource_Server_1.Configurations;
public static class SwaggerConfig
{
    private const string SwaggerSpecUrl = "swagger/v1/swagger.json";
    //const string outputFolder = "{ProjectName}.Client/src/resources/api-clients/";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var identityHost = configuration["IdentityHost"] ;
        var identityServerUrl = string.IsNullOrEmpty(identityHost) ? "https://localhost:7000" : identityHost;
        
        services.AddEndpointsApiExplorer();
        
        services.AddOpenApiDocument(options =>
        {
            
            options.AddSecurity("oauth2",  new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Name = "Authorization",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = $"{identityServerUrl}/connect/authorize",
                        TokenUrl = $"{identityServerUrl}/connect/token",
                        Scopes = new Dictionary<string, string>
                        {
                            { "api1", "resource server scope" },
                            { "read", "Read access to protected resources"  },
                            { "write", "Write access to protected resources" },
                            { "delete", "Delete access to protected resources" }
                        }
                    }
                }
                
            });

            options.OperationProcessors.Add(
                new OperationSecurityScopeProcessor("oauth2"));
            
            options.PostProcess = document =>
            {
                document.Info = new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "An ASP.NET Core Web API for managing ToDo items",
                    TermsOfService = "https://example.com/terms",
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = "https://example.com/contact"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = "https://example.com/license"
                    }
                };
            };
        });

        return services;
    }

    public static IApplicationBuilder UseSwagger(this WebApplication app)
    {
        
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "swagger-client",
                    ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C205",
                    UsePkceWithAuthorizationCodeGrant = true
                };
            });
            app.UseReDoc();
        }
        
        return app;
    }
}
