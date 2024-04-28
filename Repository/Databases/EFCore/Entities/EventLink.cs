using Core.Boundaries;

namespace Repository
{
    internal class EventLink
    {
        internal ulong Id { get; set; }
        internal ulong UserId { get; init; }
        internal User User { get; init; }
        internal ulong EventId { get; init; }
        internal Event? Event { get; init; }
        internal DateTimeOffset Time { get; init; }
        internal EventBond Type { get; set; }
    }
}
