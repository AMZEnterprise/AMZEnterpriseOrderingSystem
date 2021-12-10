using System.Collections.Generic;

namespace AMZEnterpriseOrderingSystem.Models.OrderDetailsViewModel
{
    public class OrderListViewModel
    {
        public IList<OrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
