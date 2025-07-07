namespace BuildingBlocks.Identity.Users.Dtos;

public class UserSummaryDto
{
    public Guid Id { get; set; }
    
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool IsActive { get; set; } 
    public bool IsOnline { get; set; }
    public virtual DateTimeOffset? LockoutEnd { get; set; }
    public bool IsLocked => LockoutEnd != null && LockoutEnd > DateTime.UtcNow;
    
    public DateTime? LastLoginOn { get; set; }
    public DateTime CreatedOn { get; set; }
    
}

public class UserOnlineDto
{
    public Guid? Id { get; set; }
    
    public string? UserName { get; set; }
    public string? Email { get; set; }

    public bool IsActive { get; set; } 
    public bool IsOnline { get; set; }
    public virtual DateTimeOffset? LockoutEnd { get; set; }
    public DateTime? LastLoginOn { get; set; }

}
