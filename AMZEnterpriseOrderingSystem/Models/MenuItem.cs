using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMZEnterpriseOrderingSystem.Models
{
    public class MenuItem
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Display(Name = "تصویر")]
        public string Image { get; set; }
        [Display(Name = "تند / معمولی")]
        public string Spicyness { get; set; }

        public enum EScipy
        {
            [Display(Name = "معمولی")]
            NA=0,
            [Display(Name = "تند")]
            Spicy = 1,
            [Display(Name = "خیلی تند")]
            VerySpicy = 2
        }

        [Range(1,int.MaxValue,ErrorMessage ="Price should be greater than ${1}")]
        [Display(Name = "قیمت")]
        public double Price { get; set; }

        [Display(Name ="دسته بندی")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }


        [Display(Name = "زیر دسته")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

    }
}
