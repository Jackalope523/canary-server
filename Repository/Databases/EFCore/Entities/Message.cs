namespace Repository
{
    public class Message : Entity
    {
        public long UserId { get; set; }
        public string Text { get; set; } = DefaultText;
        public string? Payload { get; set; }

        // Navigation Properties
        public User? User { get; set; }

        // Default Values
        public static string DefaultText { get; set; } = "";
    }
}
