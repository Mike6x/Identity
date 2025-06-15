using Identity.Core.Settings;
using Identity.Shared.Authorization;

namespace Identity.Infrastructure.Data.Workers;

public class UserCollection
{
    private readonly List<UserConfig> _users = [];
   
    public IEnumerable<UserConfig> GetAll() => _users;

    public UserCollection()
    {
        _users.Add(new UserConfig
        {
            Username = TenantConstants.Root.Name, 
            Email = TenantConstants.Root.EmailAddress , 
            Password = TenantConstants.DefaultPassword,
            Role = AppRoles.Superuser
        });
        
        _users.Add(new UserConfig
        {
            Username = "RootAdmin", 
            Email = "radmin@root.com" , 
            Password = "P@ssw0rd",
            Role = AppRoles.Root
        });
        
        _users.Add(new UserConfig
        {
            Username = "ManagerTest", 
            Email = "managerTest@example.com" , 
            Password = "P@ssw0rd",
            Role = AppRoles.Manager
        });
        
        _users.Add(new UserConfig
        {
            Username = "EditorTest", 
            Email ="editorTest@example.com" , 
            Password = "P@ssw0rd",
            Role = AppRoles.Editor
        });
        
        _users.Add(new UserConfig
        {
            Username = "CustomerTest", 
            Email ="customerTest@example.com" , 
            Password = "P@ssw0rd",
            Role = AppRoles.Customer
        });
        
        _users.Add(new UserConfig
        {
            Username = "ViewerTest", 
            Email ="ViewerTest@example.com" , 
            Password = "P@ssw0rd",
            Role = AppRoles.Viewer
        });
        
    }
}

