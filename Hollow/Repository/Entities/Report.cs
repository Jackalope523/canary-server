
namespace Repository
{
    public abstract class Report
    {
        public Guid Id { get; init; }

        public Guid SelfId { get; init; }
        internal User Self { get; init; } // Navigation Property

        public Guid OtherId { get; init; }
        internal User Other { get; init; } // Navigation Property

        public Guid EventId { get; init; } 
        internal Event Event { get; init; } // Navigation Property

        public DateTimeOffset FilingDate { get; init; }
        public string Notes { get; init; }           
    }
}
