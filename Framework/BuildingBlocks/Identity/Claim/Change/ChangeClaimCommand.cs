namespace Identity.Core.Features.Claim.Change
{
    public class ChangeClaimCommand
    {
        public string Owner { get; set; } = string.Empty;

        public ClaimViewModel Original { get; set; } = new ClaimViewModel();

        public ClaimViewModel Modified { get; set; } = new ClaimViewModel();

    }
}
