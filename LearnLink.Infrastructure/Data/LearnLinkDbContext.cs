﻿using LearnLink.Infrastructure.Data.Models;
using LearnLink.Infrastructure.Data.SeedDbContext;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnLink.Infrastructure.Data
{
    public class LearnLinkDbContext : IdentityDbContext<ApplicationUser>
    {
        public LearnLinkDbContext(DbContextOptions<LearnLinkDbContext> options)
            : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectConfiguration());
            modelBuilder.ApplyConfiguration(new GradeConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}