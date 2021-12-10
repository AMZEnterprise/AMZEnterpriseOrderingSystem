using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models.SubCategoryViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public SubCategory SubCategory { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }

        public List<string> SubCategoryList { get; set; }

        [Display(Name ="زیر دسته جدید")]
        public bool isNew { get; set; }

        public string StatusMessage { get; set; }

    }
}
