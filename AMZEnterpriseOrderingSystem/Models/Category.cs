using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name="نام دسته بندی")]
        public string Name { get; set; }


        [Required]
        [Display(Name="Display Order")]
        public int DisplayOrder { get; set; }
    }
}
