using System.ComponentModel.DataAnnotations;

namespace Identity.Admin.Components.OpenIddict.ViewModels
{
    public  class DisableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Code is required to disable 2FA")]
        public string Code { get; set; } = string.Empty;
    }
}
