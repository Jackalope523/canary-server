namespace Repository
{
    public class Banner
    {
        public ulong Id { get; set; }

        public string Name { get; set; } = DefaultName;
        public string Description { get; set; } = DefaultDescription;

        // Navigation Properties
        public List<GatheringLink>? Links { get; set; }

        // Default Values
        public static ulong DefaultId { get; set; } = ulong.MinValue;

        public static string DefaultName { get; set; } = "";
        public static string DefaultDescription { get; set; } = "";
    }
}
