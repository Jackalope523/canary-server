namespace Repository
{
    public class BannerLink
    {
        public ulong Id { get; set; }

        public ulong UserId { get; set; }
        public ulong BannerId { get; set; }
        public DateTimeOffset Time { get; set; } = DefaultTime;

        // Navigation Properties
        public User? User { get; set; }
        public Banner? Banner { get; set; }

        // Default Values
        public static DateTimeOffset DefaultTime { get; set; } = DateTimeOffset.MinValue;
    }
}