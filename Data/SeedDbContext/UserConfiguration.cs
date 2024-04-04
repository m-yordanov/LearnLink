using LearnLink.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnLink.Data.SeedDbContext
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
