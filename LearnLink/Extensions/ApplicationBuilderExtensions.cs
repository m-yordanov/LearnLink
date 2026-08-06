using LearnLink.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using static LearnLink.Core.Constants.RoleConstants;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        private const string DefaultAdminEmail = "admin@mail.com";

        private const string DefaultAdminPassword = "3Z4ZSLc1jTXxYiD";


        public static async Task SeedRoles(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = new[] { AdminRole, TeacherRole, StudentRole };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdmin(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                var email = configuration["AdminUser:Email"] ?? DefaultAdminEmail;

                var admin = await userManager.FindByEmailAsync(email);

                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = "The",
                        LastName = "Admin"
                    };

                    var password = configuration["AdminUser:Password"] ?? DefaultAdminPassword;

                    var createResult = await userManager.CreateAsync(admin, password);

                    if (!createResult.Succeeded)
                    {
                        logger.LogError("Failed to create the admin user '{Email}': {Errors}",
                            email,
                            string.Join("; ", createResult.Errors.Select(e => e.Description)));

                        return;
                    }

                    logger.LogInformation("Created the admin user '{Email}'.", email);
                }

                if (await userManager.IsInRoleAsync(admin, AdminRole))
                {
                    return;
                }

                var roleResult = await userManager.AddToRoleAsync(admin, AdminRole);

                if (!roleResult.Succeeded)
                {
                    logger.LogError("Failed to add '{Email}' to the {Role} role: {Errors}",
                        email,
                        AdminRole,
                        string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
