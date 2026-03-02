using System;
using System.Threading.Tasks;
using brevet_tracker.Server.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace brevet_tracker.Server.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

            try
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                const string adminRole = "Admin";
                const string adminEmail = "admin@brevet.local";
                const string adminPassword = "Admin123!";

                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError(
                            "Failed creating '{Role}' role: {Errors}",
                            adminRole,
                            string.Join("; ", roleResult.Errors));
                        return;
                    }
                }

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (!createResult.Succeeded)
                    {
                        logger.LogError(
                            "Failed creating admin user '{Email}': {Errors}",
                            adminEmail,
                            string.Join("; ", createResult.Errors));
                        return;
                    }
                }

                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                    if (!addToRoleResult.Succeeded)
                    {
                        logger.LogError(
                            "Failed assigning '{Role}' role to '{Email}': {Errors}",
                            adminRole,
                            adminEmail,
                            string.Join("; ", addToRoleResult.Errors));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding identity data.");
            }
        }
    }
}
