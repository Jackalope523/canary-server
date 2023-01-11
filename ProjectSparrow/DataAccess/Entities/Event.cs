using NetTopologySuite.Geometries;

namespace DataAccess.Entities
{
    public class Event : Entity
    {
        public Guid Id { get; set; }

        internal string Name { get; set; }
        internal string EventType { get; set; }
        internal DateTime StartTime { get; set; }
        internal Guid HostId {  get; set; }
        internal User Host { get; set; }
        public Point Location { get; set; } // X = Longitude Y = Latitude
         

        internal List<EventLink> Links { get; set; }       
    }
}
