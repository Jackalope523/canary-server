using NetTopologySuite.Geometries;
using Core.Boundaries;

namespace Repository
{
    public class Event
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public Guid HostId { get; set; }
        internal User Host { get; set; }
        public Point Location { get; set; } // X = Longitude Y = Latitude

        public bool IsEventOpen { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }
        public DateTimeOffset? EndTime { get; set; }

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
