namespace DataAccess.Entities
{
    internal class EventLink : Link
    {
        internal enum EventLinkType { Attending, Hosting, Watching, Left}

        public int EventId { get; init; }
        internal Event Event { get; init; }
        internal EventLinkType Type { get; set; }
        internal DateTime Time { get; init; }

        
      
        
    }
}
