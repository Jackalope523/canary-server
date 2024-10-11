namespace Repository
{
    public class BannerLink
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public long BannerId { get; set; }
        public DateTimeOffset Time { get; set; } = DefaultTime;

        // Navigation Properties
        public User? User { get; set; }
        public Banner? Banner { get; set; }

        // Default Values
        public static DateTimeOffset DefaultTime { get; set; } = DateTimeOffset.MinValue;
    }
}