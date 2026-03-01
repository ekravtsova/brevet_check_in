using System.Collections.Generic;

namespace brevet_tracker.Server.Models
{
    public class Checkpoint : BaseEntity
    {
        public int BrevetId { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Order { get; set; }

        public int? MaxTimeLimit { get; set; }

        public Brevet Brevet { get; set; } = null!;

        public ICollection<UserMark> UserMarks { get; set; } = new List<UserMark>();
    }
}
