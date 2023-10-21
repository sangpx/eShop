using DocumentFormat.OpenXml.Office2016.Excel;
using eShop.Application.Systems.Users;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
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
            if (string.IsNullOrEmpty(resultToken.ResultObj))
            {
                return BadRequest("UserName or Password is incorrect!");
            }
            return Ok(resultToken);
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
            if (!result.IsSuccessed)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        //PUT: http://localhost/api/users/id
        //Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateAsync(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        //https://localhost:port/api/users/paging?pageIndex=1&pageSize=7&keyWord=..
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            var users = await _userService.GetUserPagingAsync(request);
            return Ok(users);
        }

        //https://localhost:port/api/users/id
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        //Assign Role
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var result = await _userService.RoleAssignAsync(id, request);
            if (!result.IsSuccessed) { return BadRequest(result); }
            return Ok(result);
        }
    }
}