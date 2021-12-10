using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name = "دلیل قفل کردن")]
        public string LockoutReason { get; set; }
    }
}
