﻿using eShop.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace eShop.Database.Configurations
{
    public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");
            //builder.HasNoKey();
            builder.Property(x => x.FirstName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Dob).IsRequired();

        }
    }
}
