using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Common
{
    public enum OrderStatus
    {
        InProgress, // Trong qua trinh
        Confirmed, // Xac nhan
        Shipping, // Dang ship
        Success, // Thanh cong
        Canceled // Huy
    }
}