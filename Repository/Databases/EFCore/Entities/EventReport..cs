using Core.Boundaries;

namespace Repository
{
    public class EventReport
    {
        public EventReportType Type { get; set; }

        public ulong Id { get; init; }

        public ulong UserId { get; init; }
        public User? Self { get; init; } // Navigation Property

        public ulong EventId { get; init; }
        public Event? Event { get; init; } // Navigation Property

        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }
    }
}
