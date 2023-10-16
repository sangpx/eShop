using eShop.ViewModels.Catalogs.ProductImages;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Products
{
    public interface IManageProductService
    {
        Task<ProductViewModel> GetById(int id, string languageId);

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest getProductPagingRequest);

        // tra ve Ma Doi Tuong vua tao
        Task<int> Create(ProductCreateRequest productCreateRequest);

        Task AddViewCount(int id);

        Task<int> Update(ProductUpdateRequest productUpdateRequest);

        Task<bool> UpdatePrice(int id, decimal newPrice);

        Task<bool> UpdateStock(int id, int addedQuantity);

        Task<int> Delete(int id);

        Task<string> SaveImage(IFormFile file);

        Task<int> AddImage(int productId, ProductImageCreateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<List<ProductImageViewModel>> GetListImages(int productId);

        Task<ProductImageViewModel> GetImageById(int imageId);

        Task<PagedResult<ProductViewModel>> GetAllById(string languageId, GetPublicProductPagingRequest getProductPagingRequest);

        Task<bool> CategoryAssignAsync(int id, CategoryAssignRequest request);

        Task<List<ProductViewModel>> GetFeaturedProducts(int take, string languageId);
    }
}