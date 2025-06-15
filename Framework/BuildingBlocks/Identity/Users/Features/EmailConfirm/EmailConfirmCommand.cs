namespace Identity.Core.Features.User.EmailConfirm
{
    public class EmailConfirmCommand
    {
        public string UserId { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Tenant { get; set; } = string.Empty;
    }
}