using Identity.Shared.Authorization;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Workers;

public class ScopeCollection
{
    private readonly List<OpenIddictScopeDescriptor> _scopes = [];

    public IEnumerable<OpenIddictScopeDescriptor> GetAllScopes() => _scopes;

    public ScopeCollection()
    {

        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "profile",
            DisplayName = "User profile",
            Description = "Access to user profile data",
            Resources = { "identity_server" }
        });
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "email",
            DisplayName = "Email",
            Description = "Access to email address",
            Resources = { "identity_server" }
        });
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "roles",
            DisplayName = "Roles",
            Description = "Access to user roles",
            Resources = { "identity_server" }
        });
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "offline_access",
            DisplayName = "offline_access scope",
        });   
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "persistence-api",
            DisplayName = "Persistence Api"
        });    
                
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "api",
            Resources = { "resource_server" }
        });
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "api1",
            DisplayName = "Api1 scope",
            Description = "Access to resource server 1",
            Resources = { "resource_server_1" }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CatalogWriteScope,
            Resources =
            {
                ClientConstants.CatalogResourceServer,
                ClientConstants.GatewayResourceServer
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CatalogReadScope,
            Resources =
            {
                ClientConstants.CatalogResourceServer,
                ClientConstants.GatewayResourceServer
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CartWriteScope,
            Resources =
            {
                ClientConstants.CartResourceServer,
                ClientConstants.GatewayResourceServer
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CartReadScope,
            Resources =
            {
                ClientConstants.CartResourceServer,
                ClientConstants.GatewayResourceServer
            }
        }); 
        
    }
}
