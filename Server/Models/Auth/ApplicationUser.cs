using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace brevet_tracker.Server.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserMark> Marks { get; set; } = new List<UserMark>();
    }
}
