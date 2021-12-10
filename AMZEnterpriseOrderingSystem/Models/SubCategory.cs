using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMZEnterpriseOrderingSystem.Models
{
    public class SubCategory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name= "نام زیر دسته")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "دسته بندی")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    } 
}
