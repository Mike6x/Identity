using Identity.Shared.Authorization;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Workers;

public class ApplicationCollection
{
    private string IdentityHost { get; set; }
    
    private readonly List<OpenIddictApplicationDescriptor> _applications = [];
    
    public IEnumerable<OpenIddictApplicationDescriptor> GetAllApplications() => _applications;

    public ApplicationCollection(string? identityHost)
    {
        IdentityHost = string.IsNullOrEmpty(identityHost) ? "https://localhost:7000" : identityHost;
        
        const string adminUiUrl = "https://localhost:7002";

        #region Sample Clients

        // blazorwasm-oidc-client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorwasm.oidc.application",

            ClientType = OpenIddictConstants.ClientTypes.Public,
            DisplayName = "BlazorWASM OIDC Standalone Client",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            RedirectUris =
            {
                new Uri("https://localhost:7004/authentication/login-callback"),
                // new Uri("https://localhost:7004/signin-oidc"),
                new Uri("https://oidcdebugger.com/debug")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7004/authentication/logout-callback"),
                // new Uri("https://localhost:7004/signout-callback-oidc"),
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles, 
                "offline_access",
                OpenIddictConstants.Permissions.Prefixes.Scope + "api1",
                OpenIddictConstants.Permissions.Prefixes.Scope + "dataEventRecords",
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        });

        // blazorweb-oidc-client
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorweb.oidc.application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "BlazorWeb Code PKCE Client",
            RedirectUris =
            {
                new Uri("https://localhost:7006/signin-oidc"),
                new Uri("https://localhost:7008/signin-oidc"),
       },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7006/signout-callback-oidc"),
                new Uri("https://localhost:7008/signout-callback-oidc"),
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Revocation,

                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
              
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api2",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api3",
                OpenIddictConstants.Permissions.Prefixes.Scope + "dataEventRecords"
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "blazor.oidc.application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "BlazorWeb server and wasm Code PKCE Client",
            RedirectUris =
            {
                new Uri("https://localhost:7006/signin-oidc"),
                new Uri("https://localhost:7008/signin-oidc"),
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7006/signout-callback-oidc"),
                new Uri("https://localhost:7008/signout-callback-oidc"),
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Revocation,

                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
              
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{AppScopes.WeatherReadScope}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{AppScopes.CityReadScope}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{AppScopes.CityWriteScope}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{AppScopes.StudentReadScope}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{AppScopes.StudentWriteScope}",
              
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        // mvc-web and razor-web client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "web.oidc.application",
            ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
            DisplayName = "MVC OIDC Client Application",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            RedirectUris =
            {
                new Uri("https://localhost:7005/signin-oidc"),
                new Uri("https://localhost:7007/signin-oidc") 
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7005/signout-callback-oidc"),
                new Uri("https://localhost:7007/signout-callback-oidc")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
   
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api1",
            },
        });
        
        #endregion
        
        #region Authorization Code Flow applications

        // postman - oidc-debugger
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "postman",
            ClientSecret = "postman-secret",
            DisplayName = "Postman Oidc Debugger",
            RedirectUris =
            {
                new Uri("https://oauth.pstmn.io/v1/callback") 
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api"
            },
            //Requirements =
            //{
            //    Requirements.Features.ProofKeyForCodeExchange
            //}
        });
        
        // authorization-oidc-application - https://oidcdebugger.com/
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "authorization-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C203",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "Postman client application",
            RedirectUris =
            {
                new Uri("https://oidcdebugger.com/debug")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://oauth.pstmn.io/v1/callback")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
            },
            //Requirements =
            //{
            //    Requirements.Features.ProofKeyForCodeExchange
            //}
        });
        
        //swagger client : authorization-oidc-application
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "swagger-client",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C205",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "Swagger client application",
            RedirectUris =
            {
                new Uri($"{IdentityHost}/authentication/oauth2-redirect.html"),
                new Uri("https://localhost:7201/swagger/oauth2-redirect.html"),
                new Uri("https://localhost:7202/swagger/oauth2-redirect.html"),
                new Uri("https://localhost:7203/swagger/oauth2-redirect.html")
            },
            PostLogoutRedirectUris =
            {
                new Uri($"{IdentityHost}/swagger"),
                new Uri("https://localhost:7201/resources"),
                new Uri("https://localhost:7202/resources"),
                new Uri("https://localhost:7203/resources")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api",
            },
            //Requirements =
            //{
            //    Requirements.Features.ProofKeyForCodeExchange
            //}

        });
        
        #endregion
        
        #region Client Credentials Flow Applications
        
        // client-credentials-oidc-application  
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "client-credentials-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C201",
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                
                OpenIddictConstants.Permissions.Endpoints.Token,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
            }
        });

        // console client
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = ClientConstants.Console,
            ClientSecret = ClientConstants.ConsoleSecret,
            DisplayName = ClientConstants.ConsoleDisplayName,
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                
                OpenIddictConstants.Permissions.Endpoints.Token,
                
                OpenIddictConstants.Permissions.ResponseTypes.Token,
                
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CatalogReadScope,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CatalogWriteScope,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CartReadScope,
                OpenIddictConstants.Permissions.Prefixes.Scope + AppScopes.CartWriteScope
            }
            
        });

        #endregion

        #region Password GrandType
        
        // password-oidc-application
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "password-oidc-application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C202",
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.Password,
                
                OpenIddictConstants.Permissions.Endpoints.Token
            }
        });
        
        #endregion
        
        #region Admin client
        
        // OpenIddict Admin UI
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ApplicationType = OpenIddictConstants.ApplicationTypes.Web,
            ClientId = "pixel-identity-ui",

            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
        
            DisplayName = "OpenIddict Admin UI",
            RedirectUris =
            {
                // new Uri($"{adminUiUrl}/signin-oidc"),
                new Uri($"{adminUiUrl}/authentication/login-callback")
            },
            PostLogoutRedirectUris =
            {
                // new Uri($"{adminUiUrl}/signout-callback-oidc")
                new Uri($"{adminUiUrl}/authentication/logout-callback")
            },

            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,

                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                "offline_access"
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        });


        
        // web-ui
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "web-ui",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "Web UI Client",
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:4200"),
                new Uri("https://localhost:3000"),
                new Uri("https://localhost:7057"),
            },
            RedirectUris =
            {
                new Uri("https://localhost:4200"),
                new Uri("https://localhost:3000"),
                new Uri("https://oauth.pstmn.io/v1/callback"),
                new Uri("https://localhost:7057/auth/callback"),
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Revocation,

                OpenIddictConstants.Permissions.ResponseTypes.Code,
                
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Scopes.Profile,
                
                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange,
            },
        });
        
        //nextjs-client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "nextjs-client",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "Openiddict Plus NextJs UI Client",
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:3000"),
            },
            RedirectUris =
            {
                new Uri("https://localhost:3000/auth/oidc"),
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Revocation,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        // react-client
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "react-client",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C2014",
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
            DisplayName = "React client application",
            RedirectUris =
            {
                new Uri("http://localhost:3000/oauth/callback")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:3000/")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
     
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}api1"
            },
            //Requirements =
            //{
            //    Requirements.Features.ProofKeyForCodeExchange
            //}

        });
        
        #endregion

        #region Api Resource Clients
                
        // service-worker : client-credentials-oidc- Api 
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "service-worker",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
            DisplayName = "CC for protected API",
            Permissions =
            {
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Prefixes.Scope + "cc",
            },
        });
        
        // api resource server
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "api.resource.server",
            ClientSecret = "api.resource.secret",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
            
        });
        
        // CatalogResource
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = ClientConstants.CatalogResource,
            ClientSecret = ClientConstants.CatalogResourceSecret,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        });
        
        // gateway resource server
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = ClientConstants.GatewayResource,
            ClientSecret = ClientConstants.GatewayResourceSecret,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        });

        #endregion
    }
}
