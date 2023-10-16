using DocumentFormat.OpenXml.InkML;
using eShop.Database.EF;
using eShop.Database.Entities;
using eShop.ViewModels.Catalogs.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Carts
{
    public class CartService : ICartService
    {
        private readonly EShopDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public CartService(EShopDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<List<Cart>> GetAllCartByUserIdAsync(Guid userId)
        {
            string cacheKey = "cart_" + userId;

            if (!_memoryCache.TryGetValue(cacheKey, out List<Cart> cartItems))
            {
                // Nếu không tìm thấy trong cache, thực hiện lấy giỏ hàng từ cơ sở dữ liệu
                cartItems = await _dbContext.Carts.Where(x => x.UserId == userId).ToListAsync();
                //Lưu dữ liệu vào cache để sử dụng cho các lần truy cập tiếp theo
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                };
                _memoryCache.Set(cacheKey, cartItems, cacheEntryOptions);
            }
            return cartItems;
        }

        public async Task<Cart> AddToCartAsync(Cart cart)
        {
            // Tạo một key duy nhất để lưu trữ giỏ hàng trong cache
            string cacheKey = "cart_" + cart.UserId;
            // Kiểm tra xem giỏ hàng đã tồn tại trong cache hay chưa
            if (!_memoryCache.TryGetValue(cacheKey, out Cart existingCart))
            {
                // Nếu không tồn tại, tạo giỏ hàng mới
                existingCart = new Cart();
            }
            // Thêm sản phẩm vào giỏ hàng
            await _dbContext.Carts.AddAsync(cart);

            // Lưu giỏ hàng vào cache với thời gian sống (expiration time)
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) // Thời gian sống của giỏ hàng
            };

            _memoryCache.Set(cacheKey, existingCart, cacheEntryOptions);
            return existingCart;
        }

        public Task<bool> RemoveFromCartAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<CartItemViewModel> UpdateToCartAsync(CartItemViewModel model, int productId)
        {
            throw new NotImplementedException();
        }
    }
}