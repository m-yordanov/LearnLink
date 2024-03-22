using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using LearnLink.Data.Models;

namespace LearnLink.Data.SeedDbContext
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
