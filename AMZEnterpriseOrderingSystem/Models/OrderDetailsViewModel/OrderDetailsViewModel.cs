using System.Collections.Generic;

namespace AMZEnterpriseOrderingSystem.Models.OrderDetailsViewModel
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetail { get; set; }
    }
}
