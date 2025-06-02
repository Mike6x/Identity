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
            DisplayName = "Apis scope for swagger clients",
            Description = "Access to all resource servers",
            Resources =
            {
                ClientConstants.ResourceServer01,
                ClientConstants.ResourceServer02,
                ClientConstants.ResourceServer03
            }
        });
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "api1",
            DisplayName = "Api1 scope",
            Description = "Access to resource server 1",
            Resources = { ClientConstants.ResourceServer01 }
        }); 
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "api2",
            DisplayName = "Api2 scope",
            Description = "Access to resource server 2",
            Resources = { ClientConstants.ResourceServer02 }
        }); 
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "api3",
            DisplayName = "Api3 scope",
            Description = "Access to resource server 3",
            Resources = { ClientConstants.ResourceServer03 }
        }); 
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "dataEventRecords",
            DisplayName = "dataEventRecords API access",
            Description = "Access to resource server 1",
            Resources = { "rs_dataEventRecordsApi" }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CatalogWriteScope,
            Resources =
            {
                ClientConstants.CatalogResource,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CatalogReadScope,
            Resources =
            {
                ClientConstants.CatalogResource,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CartWriteScope,
            Resources =
            {
                ClientConstants.CartResource,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CartReadScope,
            Resources =
            {
                ClientConstants.CartResource,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.WeatherReadScope,
            Resources =
            {
                ClientConstants.ResourceServer02,
                ClientConstants.ResourceServer03,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.EmployeeWriteScope,
            Resources =
            {
                ClientConstants.ResourceServer01,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.EmployeeReadScope,
            Resources =
            {
                ClientConstants.ResourceServer01,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.StudentWriteScope,
            Resources =
            {
                ClientConstants.ResourceServer03,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.StudentReadScope,
            Resources =
            {
                ClientConstants.ResourceServer03,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CityWriteScope,
            Resources =
            {
                ClientConstants.ResourceServer03,
                ClientConstants.GatewayResource
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CityReadScope,
            Resources =
            {
                ClientConstants.ResourceServer03,
                ClientConstants.GatewayResource
            }
        }); 
        
    }
}
