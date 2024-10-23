namespace Repository.Entities
{
    public class Subscription
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long UserId { get; set; }
        public string DeviceToken { get; set; }

        // Navigation Properties
        public User? User { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
