using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.API.Workers;

// [RegisterHostedService]
public class OpenIdDictWorker(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Database
            .MigrateAsync(cancellationToken: cancellationToken);

        await AddScopes(scope,cancellationToken);
        await CreateApplicationsAsync(scope, cancellationToken);
        await CreateUsersAsync(scope, cancellationToken);
    }

    private async Task CreateApplicationsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var applications = configuration.GetSection("OpenIdDict:ApplicationConfigs").Get<IEnumerable<ApplicationConfig>>();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        foreach (var applicationConfig in applications)
        {
            var client = await manager.FindByClientIdAsync(applicationConfig.ClientId, cancellationToken);
            if(client != null)
            {
                await manager.DeleteAsync(client, cancellationToken);
                client = null;
            }

            if (client == null)
            {
                var app = new OpenIddictApplicationDescriptor
                {
                    ClientId = applicationConfig.ClientId,
                    DisplayName = applicationConfig.Name,
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + applicationConfig.Scope,
                        Permissions.Prefixes.Scope + Scopes.OfflineAccess,
                    },
                    ClientType = string.IsNullOrWhiteSpace(applicationConfig.ClientSecret) ? ClientTypes.Public : ClientTypes.Confidential,
                    ClientSecret = string.IsNullOrWhiteSpace(applicationConfig.ClientSecret) ? null : applicationConfig.ClientSecret,

                };
                if(applicationConfig.PKCE)
                {
                    app.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
                }
                if(applicationConfig.RedirectUri != null)
                    foreach (var uri in applicationConfig.RedirectUri)
                    {
                        app.RedirectUris.Add(new Uri(uri));
                    }

                if (applicationConfig.PostLogoutRedirectUri != null)
                    foreach (var uri in applicationConfig.PostLogoutRedirectUri)
                    {
                        app.PostLogoutRedirectUris.Add(new Uri(uri));
                    }


                await manager.CreateAsync(app, cancellationToken);
            }

        }
        
        var client1 = await manager.FindByClientIdAsync("blazorwasm-oidc-application", cancellationToken);
        if (client1 != null)
        {
            await manager.DeleteAsync(client1, cancellationToken);
        }

        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorwasm-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "BlazorWasm Application",
            RedirectUris =
            {
                    
                new Uri("https://localhost:7002/authentication/login-callback")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7002/authentication/logout-callback")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
            },
        }, cancellationToken);

    }

    private async Task CreateUsersAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var users = configuration.GetSection("OpenIddict:Users").Get<IEnumerable<UserConfig>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        var userEmailStore = userStore as IUserEmailStore<ApplicationUser>;
        foreach (var userConfig in users ?? Enumerable.Empty<UserConfig>())
        {
            var user = await userManager.FindByEmailAsync(userConfig.Email);
            if (!string.IsNullOrWhiteSpace(userConfig.Email))
            {
               user = new ApplicationUser
                {
                    UserName = userConfig.Username,
                    Email = userConfig.Email,
                    EmailConfirmed = true
                };
                if (string.IsNullOrWhiteSpace(userConfig.Password))
                {
                    userConfig.Password = Guid.NewGuid().ToString();
                    //add 3 random upper case letters
                    userConfig.Password += new string(Enumerable.Range(0, 3).Select(_ => (char)Random.Shared.Next('A', 'Z')).ToArray());
                    Console.WriteLine($"Creating user {userConfig.Email} with password '{userConfig.Password}'");
                }
                await userManager.CreateAsync(user, userConfig.Password);
                Console.WriteLine($"Creating user {userConfig.Email}");
            }
        }
    }

    private async Task AddScopes(IServiceScope scope, CancellationToken cancellationToken)
    {

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var apiScope = await manager.FindByNameAsync("api1");
        if (apiScope != null) await manager.DeleteAsync(apiScope);

        await manager.CreateAsync(new OpenIddictScopeDescriptor
        {
            DisplayName = "Api scope",
            Name = "api1",
            Resources =
            {
                "resource_server_1"
            }
        }, cancellationToken);
    }


    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
