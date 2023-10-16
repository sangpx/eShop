using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Sales
{
    public class GetManageOrderPagingRequest : PagedResultBase
    {
        public string Keyword { get; set; }
        public string? SortOption { get; set; }
    }
}