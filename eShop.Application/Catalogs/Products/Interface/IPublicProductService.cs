using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Products.Interface
{
    public interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllById(string languageId, GetPublicProductPagingRequest getProductPagingRequest);
    }
}
