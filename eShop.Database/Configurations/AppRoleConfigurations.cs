using eShop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace eShop.Database.Configurations
{
    public class AppRoleConfigurations : IEntityTypeConfiguration<AppRole>
    {

        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.ToTable("AppRoles");
            //builder.HasNoKey();
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
        }
    }
}
