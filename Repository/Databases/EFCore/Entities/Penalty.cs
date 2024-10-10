
using Core.Boundaries;

namespace Repository.Entities
{
    public class Penalty
    {
        public ulong Id { get; set; }
        public ulong PenalizedId { get; set; }
        public PenaltyType Type { get; set; }   
        public DateTimeOffset Time { get; set; }

        // Navigation Properties
        public User? Penalized { get; set; }
    }
}
