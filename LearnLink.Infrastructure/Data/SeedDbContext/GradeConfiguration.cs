using LearnLink.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Infrastructure.Data.SeedDbContext
{
    internal class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.Property(g => g.Value).IsRequired().HasColumnType("decimal(18, 2)");

            builder.HasOne(g => g.Student)
                    .WithMany(s => s.Grades)
                    .HasForeignKey(g => g.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Subject)
                   .WithMany()
                   .HasForeignKey(g => g.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Teacher)
                   .WithMany()
                   .HasForeignKey(g => g.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            var data = new SeedData();

            builder.HasData(new Grade[] { data.FirstGrade, data.SecondGrade });
        }
    }
}
