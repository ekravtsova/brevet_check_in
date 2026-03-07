using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace brevet_tracker.Server.Models.Auth
{
    /// <summary>
    /// Application user entity used by ASP.NET Core Identity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastTokenRequestTime { get; set; }

        /// <summary>
        /// Optional navigation to Identity join rows in AspNetUserRoles.
        /// Useful for queries that need explicit role relationship loading.
        /// </summary>
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();

        public ICollection<UserMark> Marks { get; set; } = new List<UserMark>();

        /// <summary>
        /// Checks whether the provided role list contains the specified role name.
        /// </summary>
        /// <param name="roles">Role names assigned to the user.</param>
        /// <param name="roleName">Role name to check.</param>
        /// <returns>True when the user has the role; otherwise, false.</returns>
        public bool HasRole(IEnumerable<string> roles, string roleName)
        {
            if (roles == null || string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            return roles.Any(role => string.Equals(role, roleName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
