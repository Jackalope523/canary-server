namespace Repository
{
    public class BannerLink
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long UserId { get; set; }
        public long BannerId { get; set; }
        public DateTimeOffset Time { get; set; } = DefaultTime;

        // Navigation Properties
        public User? User { get; set; }
        public Banner? Banner { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
        public static DateTimeOffset DefaultTime { get; set; } = DateTimeOffset.MinValue;
    }
}