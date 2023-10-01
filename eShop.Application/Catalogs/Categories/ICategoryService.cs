using eShop.ViewModels.Catalogs.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Catalogs.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllAsync(string languageId);

        Task<CategoryViewModel> GetByIdAsync(string languageId, int id);

        Task<int> CreateAsync(CategoryCreateRequest request);

        Task<int> UpdateAsync(CategoryUpdateRequest request);

        Task<int> DeleteAsync(int id);
    }
}