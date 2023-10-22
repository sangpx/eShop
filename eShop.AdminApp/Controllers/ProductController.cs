using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using eShop.AdminApp.Services.Implement;
using eShop.ViewModels.Catalogs.Products;
using eShop.AdminApp.Services.Interface;
using eShop.Utilities.Constants;
using Microsoft.AspNetCore.Http;

namespace eShop.AdminApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProductAPIClient _productAPIClient;

        public ProductController(IConfiguration configuration, IProductAPIClient productAPIClient)
        {
            _configuration = configuration;
            _productAPIClient = productAPIClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var request = new GetManageProductPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                LanguageId = languageId
            };
            var data = await _productAPIClient.GetPagingsCallAsync(request);

            return View(data);
        }

        ////Create
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        ////Create
        //[HttpPost]
        //public async Task<IActionResult> Create(RegisterRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    var result = await _userAPIClient.CreateCallAsync(request);
        //    if (result.IsSuccessed)
        //    {
        //        TempData["result"] = "Thêm mới thành công!";
        //        return RedirectToAction("Index");
        //    }

        //    ModelState.AddModelError("", result.Message);
        //    return View(request);
        //}

        ////Update
        //[HttpGet]
        //public async Task<IActionResult> Update(Guid id)
        //{
        //    var user = await _userAPIClient.GetByIdCallAsync(id);
        //    return View();
        //}

        ////Update
        //[HttpPost]
        //public async Task<IActionResult> Update(UserUpdateRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    var result = await _userAPIClient.UpdateCallAsync(request.Id, request);
        //    if (result.IsSuccessed)
        //    {
        //        TempData["result"] = "Cập nhật thành công!";
        //        return RedirectToAction("Index");
        //    }
        //    ModelState.AddModelError("", result.Message);
        //    return View(request);
        //}
    }
}