namespace DataAccess.Entities
{
    public class EventLink : Link
    {
        internal enum EventLinkType { Attending, Watching, Left}

        public Guid EventId { get; init; }
        internal Event Event { get; init; }
        internal EventLinkType Type { get; set; }
        internal DateTimeOffset Time { get; init; }

        
      
        
    }
}
