
using eShop.Application.Catalogs.Products.Interface;
using eShop.Database.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShop.ViewModels.Common;
using eShop.ViewModels.Catalogs.Products;

namespace eShop.Application.Catalogs.Products.Implement
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _dbContext;

        public PublicProductService(EShopDbContext dbContext)
        {
            this._dbContext = dbContext;
        }



        public async Task<PagedResult<ProductViewModel>> GetAllById(string languageId, GetPublicProductPagingRequest getProductPagingRequest)
        {
            //1. Select join 
            var query = from p in _dbContext.Products
                        join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _dbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in _dbContext.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };

            //2. filter
            if (getProductPagingRequest.CategoryId.HasValue && getProductPagingRequest.CategoryId.Value > 0)
            {
                query = query.Where(x => x.pic.CategoryId == getProductPagingRequest.CategoryId);
            }

            //3. paging
            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((getProductPagingRequest.PageIndex - 1) * getProductPagingRequest.PageSize)
                .Take(getProductPagingRequest.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    Details = x.pt.Details,
                    Description = x.pt.Description,
                    DateCreated = x.p.DateCreated,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.p.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();
            //4. select and projection
            var pageResult = new PagedResult<ProductViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };
            return pageResult;
        }
    }
}
