using Identity.Shared.Authorization;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Infrastructure.Data.Workers;

public class ApplicationCollection
{
    private string IdentityHost { get; set; }
    
    private readonly List<OpenIddictApplicationDescriptor> _applications = [];
    
    public IEnumerable<OpenIddictApplicationDescriptor> GetAll() => _applications;

    public ApplicationCollection(string? identityHost)
    {
        IdentityHost = string.IsNullOrEmpty(identityHost) ? "https://localhost:7000" : identityHost;
        
        const string adminUiUrl = "https://localhost:7002";

        #region Sample Clients

        // blazorwasm-oidc-client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorwasm.oidc.application",

            ClientType = ClientTypes.Public,
            DisplayName = "BlazorWASM OIDC Standalone Client",
            ConsentType = ConsentTypes.Explicit,

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
                Permissions.GrantTypes.AuthorizationCode,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                
                Permissions.ResponseTypes.Code,

                Permissions.Scopes.Profile,
                Permissions.Scopes.Email,
                Permissions.Scopes.Roles, 
                "offline_access",
                Permissions.Prefixes.Scope + "api1",
                Permissions.Prefixes.Scope + "dataEventRecords",
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        });

        // blazorweb-oidc-client
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorweb.oidc.application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
            ConsentType = ConsentTypes.Explicit,
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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Revocation,

                Permissions.ResponseTypes.Code,

                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
              
                $"{Permissions.Prefixes.Scope}api1",
                $"{Permissions.Prefixes.Scope}api2",
                $"{Permissions.Prefixes.Scope}api3",
                Permissions.Prefixes.Scope + "dataEventRecords"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "blazor.oidc.application",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C206",
            ConsentType = ConsentTypes.Explicit,
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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Revocation,

                Permissions.ResponseTypes.Code,

                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
              
                $"{Permissions.Prefixes.Scope}api",
                $"{Permissions.Prefixes.Scope}{AppScopes.WeatherReadScope}",
                $"{Permissions.Prefixes.Scope}{AppScopes.CityReadScope}",
                $"{Permissions.Prefixes.Scope}{AppScopes.CityWriteScope}",
                $"{Permissions.Prefixes.Scope}{AppScopes.StudentReadScope}",
                $"{Permissions.Prefixes.Scope}{AppScopes.StudentWriteScope}",
              
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        // mvc-web and razor-web client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "web.oidc.application",
            ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
            DisplayName = "MVC OIDC Client Application",
            ConsentType = ConsentTypes.Explicit,

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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
   
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                
                Permissions.ResponseTypes.Code,

                Permissions.Scopes.Profile,
                Permissions.Scopes.Email,
                Permissions.Scopes.Roles,
                Permissions.Prefixes.Scope + "api1",
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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.ClientCredentials,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                Permissions.Prefixes.Scope + "api"
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
            ConsentType = ConsentTypes.Explicit,
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
                Permissions.GrantTypes.AuthorizationCode,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}api1"
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
            ConsentType = ConsentTypes.Explicit,
            DisplayName = "Swagger client application",
            RedirectUris =
            {
                new Uri($"{IdentityHost}/swagger/oauth2-redirect.html"),
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
                Permissions.GrantTypes.AuthorizationCode,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}api",
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
                Permissions.GrantTypes.ClientCredentials,
                
                Permissions.Endpoints.Token,
                $"{Permissions.Prefixes.Scope}api1"
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
                Permissions.GrantTypes.ClientCredentials,
                
                Permissions.Endpoints.Token,
                
                Permissions.ResponseTypes.Token,
                
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                Permissions.Prefixes.Scope + AppScopes.CatalogReadScope,
                Permissions.Prefixes.Scope + AppScopes.CatalogWriteScope,
                Permissions.Prefixes.Scope + AppScopes.CartReadScope,
                Permissions.Prefixes.Scope + AppScopes.CartWriteScope
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
                Permissions.GrantTypes.Password,
                
                Permissions.Endpoints.Token
            }
        });
        
        #endregion
        
        #region Admin client
        
        // OpenIddict Admin UI
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ApplicationType = ApplicationTypes.Web,
            ClientId = "pixel-identity-ui",

            ClientType = ClientTypes.Public,
            ConsentType = ConsentTypes.Implicit,
        
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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,

                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                "offline_access"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        });


        
        // web-ui
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "web-ui",
            ConsentType = ConsentTypes.Explicit,
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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Revocation,

                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Email,
                Permissions.Scopes.Roles,
                Permissions.Scopes.Profile,
                
                Permissions.Prefixes.Scope + "api",
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange,
            },
        });
        
        //nextjs-client
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = "nextjs-client",
            ConsentType = ConsentTypes.Explicit,
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
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Revocation,
                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Email,
                Permissions.Scopes.Roles,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + "api",
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }

        });
        
        // react-client
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "react-client",
            ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C2014",
            ConsentType = ConsentTypes.Explicit,
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
                Permissions.GrantTypes.AuthorizationCode,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Token,
     
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                $"{Permissions.Prefixes.Scope}api1"
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
                Permissions.GrantTypes.ClientCredentials,
                
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Prefixes.Scope + "cc",
            },
        });
        
        // api resource server
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = "api.resource.server",
            ClientSecret = "api.resource.secret",
            Permissions =
            {
                Permissions.Endpoints.Introspection
            }
            
        });
        
        // CatalogResource
        _applications.Add(new OpenIddictApplicationDescriptor
        {
            ClientId = ClientConstants.CatalogResource,
            ClientSecret = ClientConstants.CatalogResourceSecret,
            Permissions =
            {
                Permissions.Endpoints.Introspection
            }
        });
        
        // gateway resource server
        _applications.Add( new OpenIddictApplicationDescriptor
        {
            ClientId = ClientConstants.GatewayResource,
            ClientSecret = ClientConstants.GatewayResourceSecret,
            Permissions =
            {
                Permissions.Endpoints.Introspection
            }
        });

        #endregion
    }
}
