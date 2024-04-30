using Core.Boundaries;

namespace Repository
{
    public class EventLink
    {
        public ulong Id { get; set; }
        public ulong UserId { get; init; }
        public User User { get; init; }
        public ulong EventId { get; init; }
        public Event? Event { get; init; }
        public DateTimeOffset Time { get; init; }
        public EventBond Type { get; set; }
    }
}
