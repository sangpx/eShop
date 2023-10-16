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
        Task<string> LoginAsync(LoginRequest request);

        Task<bool> RegisterAsync(RegisterRequest request);

        //Lay da ds user va tra ve Model phan trang
        Task<PagedResult<UserViewModel>> GetUserPagingAsync(GetUserPagingRequest request);

        //Them User
        //Chinh sua User
        //Xoa User

        Task<bool> RoleAssignAsync(Guid id, RoleAssignRequest request);
    }
}