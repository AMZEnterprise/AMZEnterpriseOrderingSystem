using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models
{
    public class Coupons
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "نوع کوپن")]
        public string CouponType { get; set; }
        public enum ECouponType { Percent=0,Dollar=1}
        [Required]
        [Display(Name = "تخفیف")]
        public double Discount { get; set; }
        [Required]
        [Display(Name = "حداقل مقدار")]
        public double MinimumAmount { get; set; }
        [Display(Name = "تصویر")]
        public byte[] Picture { get; set; }
        [Display(Name = "وضعیت")]
        public  bool isActive { get; set; }

    }
}
