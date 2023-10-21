using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using System;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Interface
{
    public interface IUserAPIClient
    {
        Task<ApiResult<string>> LoginCallAsync(LoginRequest request);

        Task<ApiResult<PagedResult<UserViewModel>>> GetUsersPagingsCallAsync(GetUserPagingRequest request);

        Task<ApiResult<bool>> CreateCallAsync(RegisterRequest request);

        Task<ApiResult<bool>> UpdateCallAsync(Guid id, UserUpdateRequest request);

        Task<ApiResult<UserViewModel>> GetByIdCallAsync(Guid id);
    }
}