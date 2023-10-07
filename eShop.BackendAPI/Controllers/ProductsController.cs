using eShop.Application.Catalogs.Products;
using eShop.ViewModels.Catalogs.ProductImages;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace eShop.BackendAPI.Controllers
{
    // api/products
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IManageProductService _manageProductService;

        public ProductsController(IManageProductService manageProductService)
        {
            _manageProductService = manageProductService;
        }

        ////https://localhost:port/api/products?pageIndex=1&pageSize=10&CategoryId=5
        //[HttpGet("{languageId}")]
        //[Authorize]
        //public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] GetPublicProductPagingRequest request)
        //{
        //    try
        //    {
        //        var data = await _manageProductService.GetAllById(languageId, request);
        //        return Ok(new ApiResponse
        //        {
        //            Success = true,
        //            Message = "Successfully!",
        //            Data = data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}

        [HttpGet("paging")]
        [Authorize]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetManageProductPagingRequest request)
        {
            try
            {
                var data = await _manageProductService.GetAllPaging(request);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Successfully!",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //https://localhost:port/api/products/{productId}/{languageId}
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById([FromQuery] int productId, string languageId)
        {
            try
            {
                var data = await _manageProductService.GetById(productId, languageId);
                if (data == null)
                {
                    return BadRequest("Cannot find product");
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Successfully!",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productId = await _manageProductService.Create(request);
                if (productId == 0)
                {
                    return BadRequest();
                }
                var product = await _manageProductService.GetById(productId, request.LanguageId);
                return Created(nameof(GetById), product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromForm] ProductUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                request.Id = productId;
                var affectedResult = await _manageProductService.Update(request);

                if (affectedResult == 0)
                {
                    return BadRequest();
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Update Successfully!",
                    Data = affectedResult
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete([FromQuery] int productId)
        {
            try
            {
                var affectedResult = await _manageProductService.Delete(productId);

                if (affectedResult == 0)
                {
                    return BadRequest();
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Delete Successfully!",
                    Data = affectedResult
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPatch("{productId}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            try
            {
                var isSuccessful = await _manageProductService.UpdatePrice(productId, newPrice);

                if (isSuccessful)
                {
                    return Ok(isSuccessful);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //https://localhost:port/api/products/{images}/{imageId}
        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            try
            {
                var image = await _manageProductService.GetImageById(imageId);
                if (image == null)
                {
                    return BadRequest("Cannot find product");
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Get Successfully!",
                    Data = image
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //Images-API
        //Create Image
        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var imageId = await _manageProductService.AddImage(productId, request);
                if (imageId == 0)
                {
                    return BadRequest();
                }
                var image = await _manageProductService.GetImageById(imageId);
                return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //Images-API
        //Update Image
        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _manageProductService.UpdateImage(imageId, request);
                if (result == 0)
                {
                    return BadRequest();
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Update Successfully!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //Images-API
        //Delete Image
        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _manageProductService.RemoveImage(imageId);
                if (result == 0)
                {
                    return BadRequest();
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Delete Successfully!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}