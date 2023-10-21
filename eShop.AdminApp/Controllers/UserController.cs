using eShop.AdminApp.Services.Implement;
using eShop.AdminApp.Services.Interface;
using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShop.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserAPIClient _userAPIClient;
        private readonly IConfiguration _configuration;

        public UserController(IUserAPIClient userAPIClient, IConfiguration configuration)
        {
            _userAPIClient = userAPIClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            var request = new GetUserPagingRequest()
            {
                KeyWord = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _userAPIClient.GetUsersPagingsCallAsync(request);
            return View(data.ResultObj);
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Create
        [HttpPost]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await _userAPIClient.CreateCallAsync(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Thêm mới thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        //Update
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var user = await _userAPIClient.GetByIdCallAsync(id);
            return View();
        }

        //Update
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await _userAPIClient.UpdateCallAsync(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        //Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Login", "User");
        }
    }
}