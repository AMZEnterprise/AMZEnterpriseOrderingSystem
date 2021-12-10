using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "تلفن")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
        [Required]
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
    }
}
