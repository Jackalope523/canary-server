namespace Repository
{
    public class Conversation : Entity
    {
        public long? GatheringId { get; set; }
        public string Title { get; set; } = DefaultTitle;
        public ConversationType Type { get; set; }

        // Navigation Properties
        public Gathering? Gathering { get; set; }
        public List<ConversationLink>? ConversationLinks { get; set; }
        public List<Message>? Messages { get; set; }

        // Default Values
        public static string DefaultTitle { get; set; } = "";

    }
}
