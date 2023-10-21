using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Systems.Users
{
    public interface IUserService
    {
        //Login
        Task<ApiResult<string>> LoginAsync(LoginRequest request);

        //Dky User
        Task<ApiResult<bool>> RegisterAsync(RegisterRequest request);

        //Chinh sua User
        Task<ApiResult<bool>> UpdateAsync(Guid id, UserUpdateRequest request);

        //Lay da ds user va tra ve Model phan trang
        Task<ApiResult<PagedResult<UserViewModel>>> GetUserPagingAsync(GetUserPagingRequest request);

        //Xoa User

        //Dky Quyen cho mot User
        Task<ApiResult<bool>> RoleAssignAsync(Guid id, RoleAssignRequest request);

        //Get User By Id
        Task<ApiResult<UserViewModel>> GetUserByIdAsync(Guid id);
    }
}