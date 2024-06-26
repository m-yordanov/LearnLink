﻿using LearnLink.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnLink.Infrastructure.Data.SeedDbContext
{
    internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser>builder)
        {
            var data = new SeedData();

            builder.HasData(new ApplicationUser[] { data.StudentUser, data.TeacherUser, data.AdminUser });
        }
    }
}
