using DocumentFormat.OpenXml.Spreadsheet;
using eShop.Database.Entities;
using eShop.Utilities.Exceptions;
using eShop.ViewModels.Catalogs.Products;
using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        //GetUserByIdAsync
        public async Task<ApiResult<UserViewModel>> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserViewModel>("User không tồn tại!");
            }
            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Dob = user.Dob
            };
            return new ApiSuccessResult<UserViewModel>(userViewModel);
        }

        //GetUserPagingAsync
        public async Task<ApiResult<PagedResult<UserViewModel>>> GetUserPagingAsync(GetUserPagingRequest request)
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
                    Id = x.Id,
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                }).ToListAsync();
            //4. select and projection
            var pageResult = new PagedResult<UserViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data,
            };
            return new ApiSuccessResult<PagedResult<UserViewModel>>(pageResult);
        }

        //LoginAsync
        public async Task<ApiResult<string>> LoginAsync(LoginRequest request)
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
            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        //RegisterAsync
        public async Task<ApiResult<bool>> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra UserName đã tồn tại chưa
            if (await _userManager.FindByNameAsync(request.UserName) != null)
            {
                return new ApiErrorResult<bool>("username đã tồn tại!");
            }

            // Kiểm tra Email đã tồn tại chưa
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Email đã tồn tại!");
            }

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
                return new ApiSuccessResult<bool>();
            }

            return new ApiErrorResult<bool>("Đăng Ký Không Thành Công");
        }

        //RoleAssignAsync
        public async Task<ApiResult<bool>> RoleAssignAsync(Guid id, RoleAssignRequest request)
        {
            //Lay ra User
            var user = await _userManager.FindByIdAsync(id.ToString());
            //Neu TH user ko ton tai
            if (user == null)
            {
                return new ApiErrorResult<bool>("username đã tồn tại!");
            }

            /* Khi gán quyền, người dùng bấm lưu lại thì kiểm tra xem role nào đã bỏ chọn
             * Sau đó lấy ra danh sách role đã bỏ chọn ( selected == false )
             * Dựa vào danh sách này sẽ tương tác với db và remove các role đã bị bỏ chọn khỏi user
             */
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
            return new ApiSuccessResult<bool>();
        }

        //UpdateAsync
        public async Task<ApiResult<bool>> UpdateAsync(Guid id, UserUpdateRequest request)
        {
            // Có bất cứ th nào thỏa mãn điều kiện có email và khác ID
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Email đã tồn tại!");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            //User co roi thi se tien hanh Update
            user.Dob = request.DateOfBirth;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }

            return new ApiErrorResult<bool>("Cập Nhật Người Dùng Không Thành Công!");
        }
    }
}