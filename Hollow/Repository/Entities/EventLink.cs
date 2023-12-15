namespace Repository
{
    public class EventLink : Link
    {
        public enum EventLinkType { Watching, Attend, Present, Left }

        internal Event Event { get; init; }
        public EventLinkType Type { get; set; }
        public DateTimeOffset Time { get; init; }

    }
}
