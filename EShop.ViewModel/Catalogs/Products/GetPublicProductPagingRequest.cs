﻿using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Products
{
    public class GetPublicProductPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }
    }
}
