using eShop.Application.Catalogs.Categories;
using eShop.ViewModels.Catalogs.Categories;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace eShop.BackendAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll(string languageId)
        {
            try
            {
                var data = await _categoryService.GetAllAsync(languageId);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Get Successfully!",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}/{languageId}")]
        public async Task<IActionResult> GetById(string languageId, int id)
        {
            try
            {
                var data = await _categoryService.GetByIdAsync(languageId, id);
                if (data == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Can't found data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Get Successfully!",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}