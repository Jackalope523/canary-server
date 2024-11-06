namespace Repository
{
    public class Banner : Entity
    {
        public string Name { get; set; } = DefaultName;
        public string Description { get; set; } = DefaultDescription;
        public string Code { get; set; } = DefaultCode;
        public string Color { get; set; } = DefaultColor;

        // Navigation Properties
        public List<BannerLink>? Links { get; set; }

        // Default Values
        public static string DefaultName { get; set; } = "";
        public static string DefaultDescription { get; set; } = "";
        public static string DefaultCode { get; set; } = "";
        public static string DefaultColor { get; set; } = "";
    }
}
