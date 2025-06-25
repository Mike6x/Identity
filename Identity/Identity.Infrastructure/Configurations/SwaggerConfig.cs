using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace Identity.Infrastructure.Configurations;
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
                            { "api1", "resource server scope" }
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
                    Title = "OpenIddict API",
                    Description = "An OpenIddict API for managing Identity items",
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
        app.UseOpenApi(config =>
        {
            
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
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
    

    // public static async Task GenerateClients(this WebApplication app)
    // {
    //     await app.GenerateDotNetClient();
    //     await app.GenerateTypescriptClient();
    // }
    //
    // private static async Task GenerateDotNetClient(this WebApplication app)
    // {
    //     var options = app.Configuration.GetSection("ClientGeneration:DotNet")?.Get<ClientGenerationOptions>();
    //     if (options?.Enabled != true)
    //         return;
    //
    //     var server = app.Services.GetService<IServer>();
    //     var addF = server?.Features.Get<IServerAddressesFeature>();
    //     var baseUrl = addF?.Addresses.FirstOrDefault();
    //
    //     if (baseUrl == null)
    //     {
    //         throw new InvalidOperationException("Failed to get base url");
    //     }
    //
    //     var uri = new Uri(new Uri(baseUrl), SwaggerSpecUrl);
    //     var document = await OpenApiDocument.FromUrlAsync(uri.AbsoluteUri);
    //
    //
    //     var settings = new CSharpClientGeneratorSettings
    //     {
    //         ClassName = "{controller}Client",
    //         ClientBaseClass = "ClientBase",
    //         GenerateClientClasses = true,
    //         GenerateClientInterfaces = false,
    //         GenerateOptionalParameters = true,
    //     };
    //
    //     var outputFolder = Environment.CurrentDirectory;
    //     if (options.ClientPath != null)
    //         outputFolder = Path.Combine(outputFolder, options.ClientPath);
    //
    //   
    //     var filePath = Path.Combine(outputFolder, $"{options.ClientName}.cs");
    //
    //     var generator = new CSharpClientGenerator(document, settings);
    //     var code = generator.GenerateFile();
    //
    //     Directory.CreateDirectory(outputFolder);
    //     File.WriteAllText(filePath, code);
    // }
    //
    // private static async Task GenerateTypescriptClient(this WebApplication app)
    // {
    //     var options = app.Configuration.GetSection("ClientGeneration:TypeScript")?.Get<ClientGenerationOptions>();
    //     if (options?.Enabled != true)
    //         return;
    //
    //
    //     var server = app.Services.GetService<IServer>();
    //     var addF = server?.Features.Get<IServerAddressesFeature>();
    //     var baseUrl = addF?.Addresses.FirstOrDefault();
    //
    //     if (baseUrl == null)
    //     {
    //         throw new InvalidOperationException("Failed to get base url");
    //     }
    //
    //     var uri = new Uri(new Uri(baseUrl), SwaggerSpecUrl);
    //     var document = await OpenApiDocument.FromUrlAsync(uri.AbsoluteUri);
    //
    //     var settings = new TypeScriptClientGeneratorSettings
    //     {
    //         ClassName = "{controller}Client",
    //         OperationNameGenerator = new MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator(),
    //         Template = TypeScriptTemplate.Fetch,
    //         TypeScriptGeneratorSettings =
    //         {
    //             TypeScriptVersion = 5.0m,
    //             //ExtensionCode = @"",
    //         },
    //         HttpClass = HttpClass.HttpClient,
    //         BaseUrlTokenName = baseUrl,
    //         InjectionTokenType = InjectionTokenType.OpaqueToken,
    //         ClientBaseClass = "ClientBase",
    //         GenerateClientClasses = true,
    //         GenerateClientInterfaces = false,
    //         GenerateOptionalParameters = true,
    //         UseGetBaseUrlMethod = true,
    //         UseTransformOptionsMethod = true,
    //         UseTransformResultMethod = true,
    //     };
    //
    //     var outputFolder = Environment.CurrentDirectory;
    //     if (options.ClientPath != null)
    //         outputFolder = Path.Combine(outputFolder, options.ClientPath);
    //
    //     if (options.Extend == true)
    //     {
    //         var extensionFilePath = Path.Combine(outputFolder, $"{options.ClientName}-extension.ts");
    //         if (File.Exists(extensionFilePath))
    //             settings.TypeScriptGeneratorSettings.ExtensionCode = await File.ReadAllTextAsync(extensionFilePath);
    //         else
    //             settings.TypeScriptGeneratorSettings.ExtensionCode = $"//Extension Not Found at {extensionFilePath}";
    //     }
    //     var filePath = Path.Combine(outputFolder, $"{options.ClientName}.ts");
    //
    //     var generator = new TypeScriptClientGenerator(document, settings);
    //     var code = generator.GenerateFile();
    //
    //     Directory.CreateDirectory(outputFolder);
    //     File.WriteAllText(filePath, code);
    // }
    //
    // private class ClientGenerationOptions
    // {
    //     public bool Enabled { get; set; } = false;
    //     public string? ClientPath { get; set; }
    //
    //     public string? ClientName { get; set; } = "client";
    //     public bool? Extend { get; set; } = false;
    //
    // }

    //class MinimalNameGenerator : IOperationNameGenerator
    //{
    //    public bool SupportsMultipleClients => throw new NotImplementedException();

    //    public string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
    //    {
    //        return "Client";
    //    }

    //    public string GetOperationName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
    //    {
    //        return "Client";
    //    }
    //}
}
