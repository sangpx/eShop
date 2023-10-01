using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Database.Entities
{
    public class CategoryTranslation
    {
        //id Cua CategoryTranslation
        public int Id { set; get; }

        //id Cua Category
        public int CategoryId { set; get; }

        public string Name { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }
        public string LanguageId { set; get; }
        public string SeoAlias { set; get; }
        public Category Category { get; set; }
        public Language Language { get; set; }
    }
}