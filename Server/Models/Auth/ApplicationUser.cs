using System;
using System.Collections.Generic;
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

        public ICollection<UserMark> Marks { get; set; } = new List<UserMark>();
    }
}
