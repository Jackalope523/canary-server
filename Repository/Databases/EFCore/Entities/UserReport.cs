using Core.Boundaries;

namespace Repository
{
    internal class UserReport
    {
        internal UserReportType Type { get; set; }

        internal ulong Id { get; init; }

        internal ulong SelfId { get; init; }
        internal User? Self { get; init; } // Navigation Property

        internal ulong OtherId { get; init; }
        internal User? Other { get; init; } // Navigation Property

        internal ulong? EventId { get; init; }
        internal Event? Event { get; init; } // Navigation Property

        internal DateTimeOffset FilingDate { get; init; }
        internal string Notes { get; init; }
    }
}
