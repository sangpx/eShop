using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Sales.Orders
{
    public class OrderDetailViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { set; get; }
        public string Name { set; get; }
        public string Category { get; set; }
        public string ThumbnailImage { get; set; }
    }
}