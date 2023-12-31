﻿using eShop.Database.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Database.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public int? ParentId { get; set; }
        public Status Status { get; set; }
        public bool IsShowOnHome { get; set; }
        public List<ProductInCategory> ProductInCategories { get; set; }

        public List<CategoryTranslation> CategoryTranslations { get; set; }
    }
}