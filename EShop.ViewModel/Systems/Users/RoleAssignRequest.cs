using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Systems.Users
{
    public class RoleAssignRequest
    {
        public SelectItem[] Roles { get; set; }
    }
}