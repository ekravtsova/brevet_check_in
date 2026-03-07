using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using brevet_tracker.Server.Constants;
using brevet_tracker.Server.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace brevet_tracker.Server.Services.Auth
{
    /// <summary>
    /// Provides role management operations for application users via ASP.NET Core Identity.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<bool> CreateRolesAsync()
        {
            try
            {
                var defaultRoles = new[] { RoleNames.Admin, RoleNames.Participant };

                foreach (var roleName in defaultRoles)
                {
                    if (await _roleManager.RoleExistsAsync(roleName))
                    {
                        _logger.LogInformation("Role '{RoleName}' already exists.", roleName);
                        continue;
                    }

                    var createResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create role '{RoleName}'. Errors: {Errors}", roleName, errors);
                        return false;
                    }

                    _logger.LogInformation("Created role '{RoleName}'.", roleName);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating default roles.");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Cannot assign role. User id is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("Cannot assign role to user {UserId}. Role name is required.", userId);
                return false;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Cannot assign role '{RoleName}'. User {UserId} was not found.", roleName, userId);
                    return false;
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogWarning("Cannot assign role '{RoleName}' to user {UserId}. Role does not exist.", roleName, userId);
                    return false;
                }

                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    _logger.LogInformation("User {UserId} is already in role '{RoleName}'.", userId, roleName);
                    return true;
                }

                var addResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addResult.Succeeded)
                {
                    var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                    _logger.LogError(
                        "Failed to assign role '{RoleName}' to user {UserId}. Errors: {Errors}",
                        roleName,
                        userId,
                        errors);
                    return false;
                }

                _logger.LogInformation("Assigned role '{RoleName}' to user {UserId}.", roleName, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning role '{RoleName}' to user {UserId}.", roleName, userId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Cannot remove role. User id is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("Cannot remove role from user {UserId}. Role name is required.", userId);
                return false;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Cannot remove role '{RoleName}'. User {UserId} was not found.", roleName, userId);
                    return false;
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogWarning("Cannot remove role '{RoleName}' from user {UserId}. Role does not exist.", roleName, userId);
                    return false;
                }

                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    _logger.LogInformation("User {UserId} is not in role '{RoleName}'. No action required.", userId, roleName);
                    return true;
                }

                var removeResult = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
                    _logger.LogError(
                        "Failed to remove role '{RoleName}' from user {UserId}. Errors: {Errors}",
                        roleName,
                        userId,
                        errors);
                    return false;
                }

                _logger.LogInformation("Removed role '{RoleName}' from user {UserId}.", roleName, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing role '{RoleName}' from user {UserId}.", roleName, userId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Cannot get user roles. User id is required.");
                return new List<string>();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Cannot get roles. User {UserId} was not found.", userId);
                    return new List<string>();
                }

                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation("Retrieved {RoleCount} roles for user {UserId}.", roles.Count, userId);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles for user {UserId}.", userId);
                return new List<string>();
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Cannot check user role. User id is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("Cannot check role for user {UserId}. Role name is required.", userId);
                return false;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Cannot check role '{RoleName}'. User {UserId} was not found.", roleName, userId);
                    return false;
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogWarning("Cannot check role '{RoleName}' for user {UserId}. Role does not exist.", roleName, userId);
                    return false;
                }

                var isInRole = await _userManager.IsInRoleAsync(user, roleName);
                _logger.LogInformation("Role membership check for user {UserId} in role '{RoleName}': {IsInRole}.", userId, roleName, isInRole);
                return isInRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking role '{RoleName}' for user {UserId}.", roleName, userId);
                return false;
            }
        }
    }
}
