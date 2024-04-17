using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using LearnLink.Infrastructure.Data.Models;

namespace LearnLink.Infrastructure.Data.SeedDbContext
{
    internal class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            var data = new SeedData();

            builder.HasData(new Subject[] {data.FirstSubject, data.SecondSubject, data.ThirdSubject });
        }
    }
}