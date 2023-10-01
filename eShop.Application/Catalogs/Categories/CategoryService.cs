using DocumentFormat.OpenXml.InkML;
using eShop.Database.EF;
using eShop.ViewModels.Catalogs.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using eShop.Database.Entities;

namespace eShop.Application.Catalogs.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _dbContext;

        public CategoryService(EShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryViewModel>> GetAllAsync(string languageId)
        {
            var query = from c in _dbContext.Categories
                        join ct in _dbContext.CategoriesTranslation on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId
                        select new { c, ct };

            return await query.Select(x => new CategoryViewModel()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                ParentId = x.c.ParentId,
            }).ToListAsync();
        }

        public async Task<CategoryViewModel> GetByIdAsync(string languageId, int id)
        {
            var query = from c in _dbContext.Categories
                        join ct in _dbContext.CategoriesTranslation on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId
                        select new { c, ct };

            return await query.Select(x => new CategoryViewModel()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                ParentId = x.c.ParentId,
            }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> CreateAsync(CategoryCreateRequest request)
        {
            var category = new Category()
            {
                SortOrder = request.SortOrder,
                IsShowOnHome = request.IsShowOnHome,
                CategoryTranslations = new List<CategoryTranslation>()
                {
                    new CategoryTranslation()
                    {
                        Name = request.Name,
                        SeoAlias = request.SeoAlias,
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId,
                    }
                }
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category.Id;
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(CategoryUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}