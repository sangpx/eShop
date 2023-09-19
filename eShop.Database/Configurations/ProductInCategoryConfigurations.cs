using eShop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace eShop.Database.Configurations
{
    public class ProductInCategoryConfigurations : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.ToTable("ProductInCategories");
            builder.HasKey(t => new { t.ProductId, t.CategoryId });
            builder
               .HasOne(t => t.Product)
               .WithMany(t => t.ProductInCategories)
               .HasForeignKey(t => t.ProductId).HasForeignKey(pc => pc.ProductId);
            builder
                .HasOne(t => t.Category)
                .WithMany(t => t.ProductInCategories)
                .HasForeignKey(t => t.CategoryId).HasForeignKey(pc => pc.CategoryId);
        }
    }
}
