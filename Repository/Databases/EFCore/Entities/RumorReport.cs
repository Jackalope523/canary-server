namespace Repository
{
    public class RumorReport : Entity
    {
        public long? UserId { get; init; }
        public long RumorId { get; init; }
        public DateTimeOffset FilingDate { get; init; } = DefaultFilingDate;
        public string Notes { get; init; } = DefaultNotes;
        public RumorReportType Type { get; set; } = DefaultType;

        // Navigation Properties
        public User? User { get; init; }
        public Rumor? Rumor { get; init; }

        // Default Values
        public static DateTimeOffset DefaultFilingDate = DateTimeOffset.MinValue;
        public static string DefaultNotes = "";
        public static RumorReportType DefaultType = RumorReportType.Other;
    }
}
