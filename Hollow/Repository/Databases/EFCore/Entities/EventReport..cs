using Shared;

namespace Repository
{
    internal class EventReport
    {
        internal EventReportType Type { get; set; }

        internal ulong Id { get; init; }

        internal ulong UserId { get; init; }
        internal User? Self { get; init; } // Navigation Property

        internal ulong EventId { get; init; }
        internal Event? Event { get; init; } // Navigation Property

        internal DateTimeOffset FilingDate { get; init; }
        internal string Notes { get; init; }
    }
}
