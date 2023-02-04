using NetTopologySuite.Geometries;

namespace DataAccess.Entities
{
    public class Event : Entity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public Guid HostId {  get; set; }
        internal User Host { get; set; }
        public Point Location { get; set; } // X = Longitude Y = Latitude

        public bool IsEventOpen { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }
        
         

        internal List<EventLink> Links { get; set; }       
    }
}
