using Core.Boundaries;

namespace Repository
{
    public class EventLink
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public User User { get; set; }
        public ulong EventId { get; set; }
        public Event? Event { get; set; }
        public DateTimeOffset Time { get; set; }
        public EventBond Type { get; set; }
    }
}
