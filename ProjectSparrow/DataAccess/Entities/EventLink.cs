namespace DataAccess.Entities
{
    internal class EventLink : Link
    {
        private int EventId { get; init; }
        private Event Event { get; init; }
        private EventLinkType Type { get; set; }

        internal EventLink(int userId, User user, int eventId, Event @event, EventLinkType type) : base(userId, user)
        {       
            EventId = eventId;
            Event = @event;
            Type = type;
        }
    }
}
