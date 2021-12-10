using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }
}
