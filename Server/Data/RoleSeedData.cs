using System;
using System.Linq;
using System.Threading.Tasks;
using brevet_tracker.Server.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace brevet_tracker.Server.Data
{
    /// <summary>
    /// Seeds default Identity roles on application startup.
    /// </summary>
    public static class RoleSeedData
    {
        /// <summary>
        /// Ensures required application roles exist in the Identity store.
        /// </summary>
        /// <param name="serviceProvider">The application service provider.</param>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RoleSeedData");

            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var requiredRoles = new[] { RoleNames.Admin, RoleNames.Participant };

                foreach (var roleName in requiredRoles)
                {
                    if (await roleManager.RoleExistsAsync(roleName))
                    {
                        logger.LogInformation("Role '{RoleName}' already exists. Skipping.", roleName);
                        continue;
                    }

                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create role '{RoleName}'. Errors: {Errors}", roleName, errors);
                        continue;
                    }

                    logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding roles.");
            }
        }
    }
}
