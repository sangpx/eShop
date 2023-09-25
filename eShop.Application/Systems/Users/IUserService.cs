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
    }
}