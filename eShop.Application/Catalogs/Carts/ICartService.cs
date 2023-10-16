using eShop.Database.Entities;
using eShop.ViewModels.Catalogs.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Carts
{
    public interface ICartService
    {
        Task<List<Cart>> GetAllCartByUserIdAsync(Guid userId);

        Task<Cart> AddToCartAsync(Cart cart);

        Task<CartItemViewModel> UpdateToCartAsync(CartItemViewModel model, int productId);

        Task<bool> RemoveFromCartAsync(int productId);
    }
}