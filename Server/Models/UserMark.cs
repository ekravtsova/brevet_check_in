using System;
using brevet_tracker.Server.Models.Auth;

namespace brevet_tracker.Server.Models
{
    public class UserMark : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public int CheckpointId { get; set; }

        public DateTime PassedAt { get; set; }

        public bool IsSynced { get; set; }

        public string DeviceInfo { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public Checkpoint Checkpoint { get; set; } = null!;
    }
}
