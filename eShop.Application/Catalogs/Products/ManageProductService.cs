using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using eShop.Application.Common;
using eShop.Database.EF;
using eShop.Database.Entities;
using eShop.Utilities.Exceptions;
using eShop.ViewModels.Catalogs.ProductImages;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace eShop.Application.Catalogs.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _dbContext;
        private readonly IStorageService _storage;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public ManageProductService(EShopDbContext dbContext, IStorageService storage)
        {
            _dbContext = dbContext;
            _storage = storage;
        }

        //Get All Product Paging
        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            //1. Select join
            //Su dung left join de gan san pham vao danh muc
            var query = from p in _dbContext.Products
                        join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _dbContext.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join c in _dbContext.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        join pi in _dbContext.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId && pi.IsDefault == true
                        select new { p, pt, pic, pi };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            }

            if (request.CategoryId != null && request.CategoryId != 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            //3. paging
            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
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
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync();
            ////4. select and projection
            var pagedResult = new PagedResult<ProductViewModel>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pagedResult;
        }

        //Get Product by Id
        public async Task<ProductViewModel> GetById(int id, string languageId)
        {
            var product = await _dbContext.Products.FindAsync(id);
            var productTranslation = await _dbContext.ProductTranslations
                .FirstOrDefaultAsync(x => x.ProductId == id && x.LanguageId == languageId);
            var categories = await (from c in _dbContext.Categories
                                    join ct in _dbContext.CategoriesTranslation on c.Id equals ct.CategoryId
                                    join pic in _dbContext.ProductInCategories on c.Id equals pic.CategoryId
                                    where pic.ProductId == id && ct.LanguageId == languageId
                                    select ct.Name).ToListAsync();
            var image = await _dbContext.ProductImages.Where(x => x.ProductId == id && x.IsDefault == true).FirstOrDefaultAsync();

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : null,
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                ThumbnailImage = image != null ? image.ImagePath : "no-image.jpg",
            };
            return productViewModel;
        }

        //Create Product
        public async Task<int> Create(ProductCreateRequest productCreateRequest)
        {
            var product = new Product()
            {
                Price = productCreateRequest.Price,
                OriginalPrice = productCreateRequest.OriginalPrice,
                Stock = productCreateRequest.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name = productCreateRequest.Name,
                        Description = productCreateRequest.Description,
                        Details = productCreateRequest.Details,
                        SeoDescription = productCreateRequest.SeoDescription,
                        SeoAlias = productCreateRequest.SeoAlias,
                        SeoTitle = productCreateRequest.SeoTitle,
                        LanguageId = productCreateRequest.LanguageId
                    }
                }
            };

            //Save Image
            if (productCreateRequest.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Image",
                        DateCreated = DateTime.Now,
                        FileSize = productCreateRequest.ThumbnailImage.Length,
                        ImagePath = await SaveImage(productCreateRequest.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product.Id;
        }

        //Update Product
        public async Task<int> Update(ProductUpdateRequest productUpdateRequest)
        {
            //Tim kiem San Pham
            var product = await _dbContext.Products.FindAsync(productUpdateRequest.Id);
            var productTranslation = await _dbContext.ProductTranslations
                .FirstOrDefaultAsync(pt =>
               pt.ProductId == productUpdateRequest.Id && pt.LanguageId == productUpdateRequest.LanguageId);
            if (product == null || productTranslation == null) throw new EShopException($"Cannot find a product with id: {productUpdateRequest.Id}!");

            productTranslation.Name = productUpdateRequest.Name;
            productTranslation.Description = productUpdateRequest.Description;
            productTranslation.SeoAlias = productUpdateRequest.SeoAlias;
            productTranslation.SeoDescription = productUpdateRequest.SeoDescription;
            productTranslation.SeoTitle = productUpdateRequest.SeoTitle;
            productTranslation.Details = productUpdateRequest.Details;

            //Save Image
            if (productUpdateRequest.ThumbnailImage != null)
            {
                var thumbnailImage = await _dbContext.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == productUpdateRequest.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = productUpdateRequest.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await SaveImage(productUpdateRequest.ThumbnailImage);
                    _dbContext.ProductImages.Update(thumbnailImage);
                }
            }
            return await _dbContext.SaveChangesAsync();
        }

        //Delete Product
        public async Task<int> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null) throw new EShopException($"Cannot find a product: {id}!");
            var images = _dbContext.ProductImages.Where(i => i.ProductId == id);
            //Xoa xong anh moi xoa den product
            foreach (var image in images)
            {
                await _storage.DeleteFileAsync(image.ImagePath);
            }

            _dbContext.Products.Remove(product);

            return await _dbContext.SaveChangesAsync();
        }

        //Add View Count
        public async Task AddViewCount(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            product.ViewCount += 1;
            await _dbContext.SaveChangesAsync();
        }

        //Update Price
        public async Task<bool> UpdatePrice(int id, decimal newPrice)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null) throw new EShopException($"Cannot find a product: {id}!");
            product.Price = newPrice;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        //Update Stock
        public async Task<bool> UpdateStock(int id, int addedQuantity)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null) throw new EShopException($"Cannot find a product: {id}!");
            product.Stock = addedQuantity;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<string> SaveImage(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storage.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }

        //Add Image
        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await SaveImage(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            await _dbContext.ProductImages.AddAsync(productImage);
            await _dbContext.SaveChangesAsync();
            return productImage.Id;
        }

        //Update Image
        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var productImage = await _dbContext.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                throw new EShopException($"Cannot find an image with id {imageId}");
            }
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await SaveImage(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }

            _dbContext.ProductImages.Update(productImage);
            return await _dbContext.SaveChangesAsync();
        }

        //Remove Image
        public async Task<int> RemoveImage(int imageId)
        {
            var productImage = await _dbContext.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                throw new EShopException($"Cannot find an image with id {imageId}");
            }
            _dbContext.ProductImages.Remove(productImage);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            return await _dbContext.ProductImages
                .Where(x => x.ProductId == productId)
                .Select(i => new ProductImageViewModel()
                {
                    Id = i.Id,
                    Caption = i.Caption,
                    DateCreated = i.DateCreated,
                    FileSize = i.FileSize,
                    IsDefault = i.IsDefault,
                    ImagePath = i.ImagePath,
                    SortOrder = i.SortOrder,
                    ProductId = i.ProductId
                }).ToListAsync();
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _dbContext.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException($"Cannot find an image with id {imageId}");
            }
            var viewImageModel = new ProductImageViewModel()
            {
                Id = image.Id,
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                IsDefault = image.IsDefault,
                ImagePath = image.ImagePath,
                SortOrder = image.SortOrder,
                ProductId = image.ProductId
            };
            return viewImageModel;
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
            var pagedResult = new PagedResult<ProductViewModel>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = getProductPagingRequest.PageIndex,
                PageSize = getProductPagingRequest.PageSize
            };
            return pagedResult;
        }

        //assign Category
        public async Task<bool> CategoryAssignAsync(int id, CategoryAssignRequest request)
        {
            //Lay ra product
            var product = await _dbContext.Products.FindAsync(id);
            //Neu TH user ko ton tai
            if (product == null)
            {
                return false;
            }

            foreach (var category in request.Categories)
            {
                var productInCategory = await _dbContext.ProductInCategories
                    .FirstOrDefaultAsync(x => x.CategoryId == int.Parse(category.Id) && x.ProductId == id);
                if (productInCategory != null && category.Selected == false)
                {
                    _dbContext.ProductInCategories.Remove(productInCategory);
                }
                else if (productInCategory == null && category.Selected == true)
                {
                    await _dbContext.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        CategoryId = int.Parse(category.Id),
                        ProductId = id
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        //GetFeatureProducts
        public async Task<List<ProductViewModel>> GetFeaturedProducts(int take, string languageId)
        {
            //1. Select join
            //Su dung left join de gan san pham vao danh muc
            var query = from p in _dbContext.Products
                        join pt in _dbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _dbContext.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join pi in _dbContext.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _dbContext.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi != null || pi.IsDefault == true)
                        && p.IsFeatured == true
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
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
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync();
            return data;
        }
    }
}