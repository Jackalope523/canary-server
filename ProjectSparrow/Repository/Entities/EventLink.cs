namespace Repository.Entities
{
    public class EventLink : Link
    {
        public enum EventLinkType { Attend, Watch, Left }

        public Guid EventId { get; init; }
        internal Event Event { get; init; }
        public EventLinkType Type { get; set; }
        internal DateTimeOffset Time { get; init; }

    }
}
