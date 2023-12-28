using NetTopologySuite.Geometries;
using Core.Boundaries;

namespace Repository
{
    public class Event
    {
        public ulong Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public ulong HostId { get; set; }
        internal User Host { get; set; }
        public Point Location { get; set; } // X = Longitude Y = Latitude

        public EventState State { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public double Radius { get; set; }
        public bool IsDynamic { get; set; }

        // Vector
        public int Extroversion { get; init; }
        public int Athleticisme { get; init; }
        public int Openness { get; init; }
        public int Chaos { get; init; }
        public int Competitiveness { get; init; }
        public int Industriousness { get; init; }
        public int NightOwl { get; init; }

        // Navigation Properties
        internal List<EventLink> Links { get; set; }
        internal List<Report> Reports { get; set; }
        internal List<Post> Posts { get; set; }
    }
}
