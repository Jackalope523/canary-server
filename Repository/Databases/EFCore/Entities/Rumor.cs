namespace Repository
{
    public class Rumor : Entity
    {
        public long? AuthorId { get; set; }
        public long RumoredGatheringId { get; set; }
        public DateTimeOffset Time { get; set; } = DefaultTime;
        public string Text { get; set; } = DefaultText;

        // Navigation Properties
        public User? Author { get; set; }
        public RumoredGathering? RumoredGathering { get; set; }
        public List<RumorReport>? RumorReports { get; set; }

        // Default Values
        public static string DefaultText = "";
        public static DateTimeOffset DefaultTime = DateTimeOffset.MinValue;

    }
}
