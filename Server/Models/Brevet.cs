using System;
using System.Collections.Generic;

namespace brevet_tracker.Server.Models
{
    public class Brevet : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public ICollection<Checkpoint> Checkpoints { get; set; } = new List<Checkpoint>();
    }
}
