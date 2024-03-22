using LearnLink.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Data.SeedDbContext
{
    internal class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            var data = new SeedData();

            builder.HasData(new Teacher[] { data.Teacher });
        }
    }
}
