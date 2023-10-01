using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Catalogs.Categories
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }
        public string LanguageId { set; get; }
        public string SeoAlias { set; get; }
        public bool IsShowOnHome { get; set; }
    }
}