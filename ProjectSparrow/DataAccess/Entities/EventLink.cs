namespace DataAccess.Entities
{
    public class EventLink : Link
    {
        internal enum EventLinkType { Attend, Watch, Left, Inappropriate, Spam, Misleading, Promotion }

        public Guid EventId { get; init; }
        internal Event Event { get; init; }
        internal EventLinkType Type { get; set; }
        internal DateTimeOffset Time { get; init; }

        
      
        
    }
}
