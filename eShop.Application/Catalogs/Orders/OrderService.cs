using eShop.Database.EF;
using eShop.ViewModels.Catalogs.Sales;
using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Orders
{
    public class OrderService : IOrderService
    {
        private readonly EShopDbContext _dbContext;

        public OrderService(EShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> CancelOrderStatusAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(CheckoutRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<OrderViewModel>> GetAllPaging(GetManageOrderPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OrderViewModel> GetOrderByIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetailViewModel> GetOrderDetailAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatusAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}