using NetTopologySuite.Geometries;
using Server.Boundaries;

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
        public DateTimeOffset? EndTime { get; set; }

        internal List<EventLink> Links { get; set; }

        public ThinEvent ToThinEvent()
		{
			return new(Id, Host.ToThinnerUser(), Name, Description, EventType,
				StartTime, Location.Y, Location.X, EndTime,
				IsEventOpen, GroupMinimum, GroupMaximum);
		}
        public ThinnerEvent ToThinnerEvent()
		{
			return new(Id, Host.ToThinnerUser(), EventType,Location.Y, Location.X);
		}
    }
}
