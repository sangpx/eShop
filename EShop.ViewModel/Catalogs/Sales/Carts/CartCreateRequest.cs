using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Sales.Carts
{
    public class CartCreateRequest
    {
        public int ProductId { set; get; }
        public int Quantity { set; get; }
        public Guid UserId { get; set; }
    }
}