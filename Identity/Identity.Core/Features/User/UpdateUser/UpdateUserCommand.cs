using BuildingBlocks.Storage.File.Features;

namespace Identity.Core.Features.User.UpdateUser;

// public class UpdateUserCommand : IRequest
public class UpdateUserCommand
{
    public string Id { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public FileUploadCommand? Image { get; set; }
    public bool DeleteCurrentImage { get; set; }

    #region My Customize
    public string? UserName { get; set; }
    public bool IsActive { get; set; }
    public bool? IsOnline { get; set; }
    public bool EmailConfirmed { get; set; }

    public Uri? ImageUrl { get; set; }

    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; } = null;
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; } = null;

    #endregion
}
