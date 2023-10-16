using eShop.ViewModels.Catalogs.Sales;
using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Orders
{
    public interface IOrderService
    {
        Task<PagedResult<OrderViewModel>> GetAllPaging(GetManageOrderPagingRequest request);

        Task<int> CreateAsync(CheckoutRequest request);

        Task<bool> UpdateOrderStatusAsync(int orderId);

        Task<bool> CancelOrderStatusAsync(int orderId);

        Task<OrderViewModel> GetOrderByIdAsync(int orderId);

        Task<OrderDetailViewModel> GetOrderDetailAsync(int orderId);
    }
}