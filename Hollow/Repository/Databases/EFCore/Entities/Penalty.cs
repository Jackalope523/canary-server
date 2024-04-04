
using Core.Boundaries;

namespace Repository.Entities
{
    internal class Penalty
    {
        internal ulong Id { get; set; }
        internal ulong PenalizedId { get; set; }
        internal User Penalized { get; set; }
        internal PenaltyType Type { get; set; }   
        internal DateTimeOffset Time { get; set; }
    }
}
