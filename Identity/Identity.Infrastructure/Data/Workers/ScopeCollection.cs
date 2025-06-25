using Identity.Shared.Authorization;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Data.Workers;

public class ScopeCollection
{
    private readonly List<OpenIddictScopeDescriptor> _scopes = [];

    public IEnumerable<OpenIddictScopeDescriptor> GetAll() => _scopes;

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
                "resource_server_1",
                "resource_server_2",
                "resource_server_3"
            }
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
            Name = "api2",
            DisplayName = "Api2 scope",
            Description = "Access to resource server 2",
            Resources = { "resource_server_2" }
        }); 
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "api3",
            DisplayName = "Api3 scope",
            Description = "Access to resource server 3",
            Resources = { "resource_server_3"}
        }); 
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = "dataEventRecords",
            DisplayName = "dataEventRecords API access",
            Description = "Access to resource server test",
            Resources = { "rs_dataEventRecordsApi" }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CatalogWriteScope,
            Resources =
            {
                ClientConstants.CatalogResource,
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CatalogReadScope,
            Resources =
            {
                ClientConstants.CatalogResource,
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CartWriteScope,
            Resources =
            {
                ClientConstants.CartResource,
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CartReadScope,
            Resources =
            {
                ClientConstants.CartResource,
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.WeatherReadScope,
            Resources =
            {
                "resource_server_2" ,
                "resource_server_3" ,
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.EmployeeWriteScope,
            Resources =
            {
                "resource_server_1",
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.EmployeeReadScope,
            Resources =
            {
                "resource_server_1",
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.StudentWriteScope,
            Resources =
            {
                "resource_server_3",
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.StudentReadScope,
            Resources =
            {
                "resource_server_3",
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CityWriteScope,
            Resources =
            {
                "resource_server_3" ,
                "gateway.resource.server"
            }
        }); 
        
        _scopes.Add( new OpenIddictScopeDescriptor
        {
            Name = AppScopes.CityReadScope,
            Resources =
            {
                "resource_server_3" ,
                "gateway.resource.server"
            }
        }); 
        
    }
}
