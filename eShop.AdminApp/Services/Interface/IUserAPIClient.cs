using eShop.ViewModels.Systems.Users;
using System.Threading.Tasks;

namespace eShop.AdminApp.Services.Interface
{
    public interface IUserAPIClient
    {
        Task<string> Login(LoginRequest request);
    }
}