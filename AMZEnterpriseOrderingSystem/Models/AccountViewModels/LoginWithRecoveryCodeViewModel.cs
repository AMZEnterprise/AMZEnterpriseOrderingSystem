using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models.AccountViewModels
{
    public class LoginWithRecoveryCodeViewModel
    {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "رمز بازیابی")]
            public string RecoveryCode { get; set; }
    }
}
