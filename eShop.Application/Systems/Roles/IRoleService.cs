using eShop.ViewModels.Systems.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Application.Systems.Roles
{
    public interface IRoleService
    {
        Task<List<RoleViewModel>> GetAllAsync();
    }
}