using System.Collections.Generic;

namespace AMZEnterpriseOrderingSystem.Models.OrderDetailsViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
