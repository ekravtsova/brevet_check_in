namespace brevet_tracker.Server.Constants
{
    /// <summary>
    /// Centralized role name constants used by authentication and authorization logic.
    /// Add new role constants here as the system grows.
    /// </summary>
    public static class RoleNames
    {
        /// <summary>
        /// Full-access role for system administration tasks, configuration, and user management.
        /// Use this role for trusted staff with elevated permissions.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Standard role for end users participating in brevet activity and related workflows.
        /// Use this role for regular authenticated users without administrative privileges.
        /// </summary>
        public const string Participant = "Participant";
    }
}
