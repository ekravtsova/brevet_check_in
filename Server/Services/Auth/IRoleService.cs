using System.Collections.Generic;
using System.Threading.Tasks;

namespace brevet_tracker.Server.Services.Auth
{
    /// <summary>
    /// Defines role management operations backed by ASP.NET Core Identity role services.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Creates default application roles if they do not already exist.
        /// </summary>
        /// <returns>True when all default roles exist or are created successfully; otherwise, false.</returns>
        Task<bool> CreateRolesAsync();

        /// <summary>
        /// Assigns the specified role to the target user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user receiving the role.</param>
        /// <param name="roleName">The role name to assign.</param>
        /// <returns>True when the role is assigned successfully; otherwise, false.</returns>
        Task<bool> AssignRoleToUserAsync(string userId, string roleName);

        /// <summary>
        /// Removes the specified role from the target user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user losing the role.</param>
        /// <param name="roleName">The role name to remove.</param>
        /// <returns>True when the role is removed successfully; otherwise, false.</returns>
        Task<bool> RemoveRoleFromUserAsync(string userId, string roleName);

        /// <summary>
        /// Gets all role names currently assigned to the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of role names assigned to the user.</returns>
        Task<IList<string>> GetUserRolesAsync(string userId);

        /// <summary>
        /// Checks whether the specified user is currently assigned to a role.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="roleName">The role name to check.</param>
        /// <returns>True if the user is in the role; otherwise, false.</returns>
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
    }
}
