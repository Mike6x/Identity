namespace Identity.Core.Features.Claim.Update;

public class AssignClaimsCommand
{
    public string Owner { get; set; } = string.Empty;
    public List<ClaimViewModel> Claims { get; set; } = [];
}