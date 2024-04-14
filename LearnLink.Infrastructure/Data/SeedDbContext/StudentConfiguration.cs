using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Infrastructure.Data.SeedDbContext
{
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            var data = new SeedData();

            builder.HasData(new Student[] { data.Student });
        }
    }
}
