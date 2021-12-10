using System.ComponentModel.DataAnnotations;
using AMZEnterpriseOrderingSystem.Utility;

namespace AMZEnterpriseOrderingSystem.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "این دستگاه را به خاطر بسپار")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
