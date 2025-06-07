namespace Identity.Core.Features.User.EmailConfirm
{
    public class EmailConfirmCommand
    {
        public string UserId { get; set; } = default!;

        public string Code { get; set; } = default!;

        public string Tenant { get; set; } = default!;
    }
}