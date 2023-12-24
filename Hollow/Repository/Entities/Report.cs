
namespace Repository
{
    public abstract class Report
    {
        public ulong Id { get; init; }

        public ulong SelfId { get; init; }
        internal User Self { get; init; } // Navigation Property

        public ulong OtherId { get; init; }
        internal User Other { get; init; } // Navigation Property

        public ulong EventId { get; init; } 
        internal Event Event { get; init; } // Navigation Property

        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }           
    }
}
