namespace Repository
{
    public class Feedback
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long? UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Comments { get; set; }

        // Navigation Properties
        public User? User { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
