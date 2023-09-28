using eShop.ViewModels.Common;
using eShop.ViewModels.Systems.Users;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Interface
{
    public interface IUserAPIClient
    {
        Task<string> LoginCallAsync(LoginRequest request);

        Task<PagedResult<UserViewModel>> GetUsersPagingsCallAsync(GetUserPagingRequest request);
    }
}