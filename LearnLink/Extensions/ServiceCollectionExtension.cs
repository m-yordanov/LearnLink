using LearnLink.Core.Interfaces;
using LearnLink.Core.Services;
using LearnLink.Infrastructure.Data;
using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IAttendanceManagementService, AttendanceManagementService>();
            services.AddScoped<IGradeManagementService, GradeManagementService>();
            services.AddScoped<IViewCommonService, ViewCommonService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHomeService, HomeService>();

            return services;
        }

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<LearnLinkDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }

        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration config)
        {
            services.
                AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<LearnLinkDbContext>();

            return services;
        }
    }
}
