using eShop.Application.Catalogs.Products.Interface;
using eShop.ViewModels.Catalogs.ProductImages;
using eShop.ViewModels.Catalogs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace eShop.BackendAPI.Controllers
{
    // api/products
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IPublicProductService _publicProductService;
        private readonly IManageProductService _manageProductService;

        public ProductsController(IPublicProductService publicProductService, IManageProductService manageProductService)
        {
            _publicProductService = publicProductService;
            _manageProductService = manageProductService;
        }

        //https://localhost:port/api/Products?pageIndex=1&pageSize=10&CategoryId=5
        [HttpGet("{languageId}")]
        [Authorize]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] GetPublicProductPagingRequest request)
        {
            var data = await _publicProductService.GetAllById(languageId, request);
            return Ok(data);
        }

        //https://localhost:port/api/Products/{productId}/{languageId}
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById([FromQuery] int productId, string languageId)
        {
            var data = await _manageProductService.GetById(productId, languageId);
            if (data == null)
            {
                return BadRequest("Cannot find product");
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (ModelState.IsValid)
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

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            var affectedResult = await _manageProductService.Update(request);

            if (affectedResult == 0)
            {
                return BadRequest();
            }
            return Ok(affectedResult);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete([FromQuery] int productId)
        {
            var affectedResult = await _manageProductService.Delete(productId);

            if (affectedResult == 0)
            {
                return BadRequest();
            }
            return Ok(affectedResult);
        }

        [HttpPatch("{productId}/{newPrice}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var isSuccessful = await _manageProductService.UpdatePrice(productId, newPrice);

            if (isSuccessful)
            {
                return Ok(isSuccessful);
            }
            return BadRequest();
        }

        //https://localhost:port/api/Products/{images}/{imageId}
        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var image = await _manageProductService.GetImageById(imageId);
            if (image == null)
            {
                return BadRequest("Cannot find product");
            }
            return Ok(image);
        }

        //Images-API
        //Create Image
        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
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

        //Images-API
        //Update Image
        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
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
            return Ok();
        }

        //Images-API
        //Delete Image
        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
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
            return Ok();
        }
    }
}