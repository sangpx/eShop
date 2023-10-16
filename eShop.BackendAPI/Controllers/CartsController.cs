using eShop.Application.Catalogs.Carts;
using eShop.Database.Entities;
using eShop.ViewModels.Catalogs.Sales;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShop.BackendAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("getallcart/{userId}")]
        public async Task<IActionResult> GetAllCartByUserId(Guid userId)
        {
            try
            {
                var cart = await _cartService.GetAllCartByUserIdAsync(userId);
                //chuyen doi cart -> cartItemViewModel
                var cartItemViewModel = new List<CartItemViewModel>();
                foreach (var item in cart)
                {
                    cartItemViewModel.Add(new CartItemViewModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Successfully!",
                    Data = cartItemViewModel
                });
            }
            catch (Exception ex)
            {
                return NotFound("Giỏ hàng không tồn tại trong cơ sở dữ liệu");
            }
        }
    }
}