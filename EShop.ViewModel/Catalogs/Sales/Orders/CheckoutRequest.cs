using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Sales.Orders
{
    public class CheckoutRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        //Danh sach cac CartItem
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }
}