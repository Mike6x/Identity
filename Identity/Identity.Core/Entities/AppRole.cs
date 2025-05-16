using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Entities;

public sealed class AppRole : IdentityRole<Guid>
{
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public AppRoleLevel Level { get; set; } = AppRoleLevel.Customer;
    
    public AppRole(string name, string? description = null)
        : base(name)
    {
        Description = description ?? $"{name} Role for Identity Server";
        CreatedAt = DateTime.UtcNow;
        
        NormalizedName = name.ToUpperInvariant();
    }
}

public enum AppRoleLevel
{
    Admin = 0,
    Manager = 1,
    Editor = 2,
    Viewer = 3,
    Customer = 4,
}