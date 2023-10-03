using eShop.Database.Entities;
using eShop.Utilities.Exceptions;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Systems.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;

        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IConfiguration config
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        //Lay ra ds user va tra ve Model phan trang
        public async Task<PagedResult<UserViewModel>> GetUserPagingAsync(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                //Tim kiem theo UserName
                query = query.Where(x => x.UserName.Contains(request.KeyWord) ||
                    x.PhoneNumber.Contains(request.KeyWord));
            }

            //3. paging
            int totalRow = await query.CountAsync();
            var data = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserViewModel()
                {
                    UserName = x.UserName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id = x.Id,
                }).ToListAsync();
            //4. select and projection
            var pageResult = new PagedResult<UserViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };
            return pageResult;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return null;

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            //Sau khi Dang Nhap thanh cong thi tao Claim
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Role, string.Join(";", roles)),
                new Claim(ClaimTypes.Name, request.UserName)
            };

            //Ma hoa Claim
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var user = new AppUser()
            {
                Dob = request.DateOfBirth,
                Email = request.Email,
                FirstName = request.FirstName,
                UserName = request.UserName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RoleAssignAsync(Guid id, RoleAssignRequest request)
        {
            //Lay ra User
            var user = await _userManager.FindByIdAsync(id.ToString());
            //Neu TH user ko ton tai
            if (user == null)
            {
                return false;
            }
            //Kiem tra Role day co ton tai ko -> check
            // Lấy danh sách các vai trò cần xóa
            var removeRole = request.Roles.Where(x => x.Selected == false).Select(x => x.Name).ToList();

            foreach (var roleName in removeRole)
            {
                // add role ma thang nao chua ton tai
                if (await _userManager.IsInRoleAsync(user, roleName) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }

            await _userManager.RemoveFromRolesAsync(user, removeRole);

            // trong TH th nao ma ton tai
            var addedRole = request.Roles.Where(x => x.Selected == true).Select(x => x.Name).ToList();
            foreach (var roleName in addedRole)
            {
                // add role ma thang nao chua ton tai
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
            return true;
        }
    }
}