using DocumentFormat.OpenXml.Office2016.Excel;
using eShop.Application.Systems.Users;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace eShop.BackendAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        //Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultToken = await _userService.LoginAsync(request);
            if (string.IsNullOrEmpty(resultToken))
            {
                return BadRequest("UserName or Password is incorrect!");
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Login Successfully!",
                Data = resultToken
            });
        }

        //Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.RegisterAsync(request);
            if (!result)
            {
                return BadRequest("Register don't successful!");
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Register Successfully!",
                Data = result
            });
        }

        //https://localhost:port/api/users/paging?PageIndex=1&PageSize=7
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            var data = await _userService.GetUserPagingAsync(request);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "GetPaging Successfully!",
                Data = data
            });
        }
    }
}