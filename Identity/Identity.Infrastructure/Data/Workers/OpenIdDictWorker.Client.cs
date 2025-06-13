using Identity.Core.Settings;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;
namespace Identity.Infrastructure.Data.Workers;

public partial class OpenIdDictWorker
{
        
    private async Task SeedClientsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var seedingList = new ApplicationCollection(configuration["IdentityHost"]).GetAll().ToList();
        
        var loadingList = configuration.GetSection("OpenIdDict:ApplicationConfigs")
                                .Get<IEnumerable<ApplicationConfig>>();
        
        foreach (var applicationConfig in loadingList ?? [])
        {
            var app = new OpenIddictApplicationDescriptor
            {
                ClientId = applicationConfig.ClientId,
                DisplayName = applicationConfig.DisplayName,
                ClientType = string.IsNullOrWhiteSpace(applicationConfig.ClientSecret)
                    ? ClientTypes.Public
                    : ClientTypes.Confidential,
                ClientSecret = string.IsNullOrWhiteSpace(applicationConfig.ClientSecret)
                    ? null
                    : applicationConfig.ClientSecret,
                Permissions =
                {
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.GrantTypes.Password,
                    
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Introspection,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,

                    Permissions.ResponseTypes.Code,

                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles, 
                    "offline_access",

                    Permissions.Prefixes.Scope + applicationConfig.Scope,
                    Permissions.Prefixes.Scope + Scopes.OfflineAccess,
                },
            };
            if (applicationConfig.PKCE)
            {
                app.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
            }

            if (applicationConfig.RedirectUri != null)
                foreach (var uri in applicationConfig.RedirectUri)
                {
                    app.RedirectUris.Add(new Uri(uri));
                }

            if (applicationConfig.PostLogoutRedirectUri != null)
                foreach (var uri in applicationConfig.PostLogoutRedirectUri)
                {
                    app.PostLogoutRedirectUris.Add(new Uri(uri));
                }
            
            seedingList.Add(app);
        }
        
        foreach (var application in seedingList)
        {
            var existApplication = await applicationManager.FindByClientIdAsync(application.ClientId ?? string.Empty, cancellationToken);
            
           #if DEBUG 
            if(existApplication != null)
            {
                await applicationManager.DeleteAsync(existApplication, cancellationToken);
                existApplication = null;
            }
            #endif

            if (existApplication == null) await applicationManager.CreateAsync(application, cancellationToken);

        }
        
        //For each application, add redirect uri to allowed origin list on default cors policy
        // var defaultCorsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);
        
        var defaultCorsPolicy = new CorsPolicy();
        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query = (apps) =>
        {
            return apps.Where(app => true).Select(s => s as OpenIddictEntityFrameworkCoreApplication)!;
        };
        await foreach (var app in applicationManager.ListAsync(query, CancellationToken.None))
        {
            var redirectUris = await applicationManager.GetRedirectUrisAsync(app, cancellationToken);
            foreach (var uri in redirectUris.Select(s => new Uri(s)))
            {
                string origin = $"{uri.Scheme}://{uri.Authority}";
                if (!defaultCorsPolicy.Origins.Contains(origin))
                {
                    defaultCorsPolicy.Origins.Add(origin);
                }
            }
        }

    }

    
}