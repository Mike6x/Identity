using System.ComponentModel.DataAnnotations;

namespace Identity.Admin.Components.OpenIddict.ViewModels
{
    public class ResetAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Code is required to reset Authenticator")]
        public string Code { get; set; } = string.Empty;
    }
}
