namespace Repository.Entities
{
    public class Penalty
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long PenalizedId { get; set; }
        public PenaltyType Type { get; set; }   
        public DateTimeOffset Time { get; set; }

        // Navigation Properties
        public User? Penalized { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
