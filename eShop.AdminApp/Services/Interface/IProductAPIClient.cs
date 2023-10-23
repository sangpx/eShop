using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Interface
{
    public interface IProductAPIClient
    {
        Task<PagedResult<ProductViewModel>> GetPagingsCallAsync(GetManageProductPagingRequest request);

        Task<bool> CreateProductCallAsync(ProductCreateRequest request);

        Task<bool> UpdateProductCallAsync(ProductUpdateRequest request);
    }
}