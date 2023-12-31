﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Sales.Carts
{
    public class CartItemViewModel
    {
        public int ProductId { set; get; }
        public int Quantity { set; get; }
        public decimal Price { set; get; }
        public decimal TotalPrice => Quantity * Price;
        public Guid UserId { get; set; }
    }
}