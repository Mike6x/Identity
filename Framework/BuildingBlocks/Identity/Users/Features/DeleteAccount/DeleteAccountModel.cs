using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Features.User.DeleteAccount
{
    public  class DeleteAccountModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
