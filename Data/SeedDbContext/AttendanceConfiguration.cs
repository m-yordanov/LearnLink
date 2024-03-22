using LearnLink.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Data.SeedDbContext
{
    internal class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasOne(a => a.Student)
                   .WithMany(s => s.Attendances)
                   .HasForeignKey(a => a.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Subject)
                   .WithMany() 
                   .HasForeignKey(a => a.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);
            var data = new SeedData();

            builder.HasData(new Attendance[] { data.FirstAttendance, data.SecondAttendance });
        }
    }
}
