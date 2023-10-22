using eShop.AdminApp.Services.Interface;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Implement
{
    public class ProductAPIClient : BaseAPIClient, IProductAPIClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ProductAPIClient(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : base(httpClientFactory, httpContextAccessor, configuration) { }

        public async Task<PagedResult<ProductViewModel>> GetPagingsCallAsync(GetManageProductPagingRequest request)
        {
            var products = await GetAsync<PagedResult<ProductViewModel>>("/api/products/paging?LanguageId=vi&PageIndex=" +
                $"{request.PageIndex}&PageSize={request.PageSize}&Keyword={request.Keyword}");
            return products;
        }
    }
}