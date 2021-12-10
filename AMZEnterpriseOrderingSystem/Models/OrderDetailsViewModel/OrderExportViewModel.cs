using System;
using System.ComponentModel.DataAnnotations;

namespace AMZEnterpriseOrderingSystem.Models.OrderDetailsViewModel
{
    public class OrderExportViewModel
    {
        [Display(Name = "تاریخ مبدا")]
        public DateTime startDate { get; set; }
        [Display(Name = "تاریخ مقصد")]
        public DateTime endDate { get; set; }
    }
}
