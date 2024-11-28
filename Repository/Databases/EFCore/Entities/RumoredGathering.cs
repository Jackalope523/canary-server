using NetTopologySuite.Geometries;

namespace Repository
{
    public class RumoredGathering : Entity
    {
        public Point Location { get; set; } = DefaultLocation; // X = Longitude Y = Latitude
        public string FriendlyLocation { get; set; } = DefaultFriendlyLocation;
        public int ConfidenceRating { get; set; } = DefaultConfidenceRating;

        // Navigation Properties
        public List<Investigation>? Investigations { get; set; }
        public List<Rumor>? Rumors { get; set; }

        // Default Values
        private static readonly CoordinateFactory Factory = new();
     
        public static Point DefaultLocation { get; set; } = Factory.Create(7.544, 53.483);
        public static string DefaultFriendlyLocation { get; set; } = "";
        public static int DefaultConfidenceRating { get; set; } = 100;
    }
}
