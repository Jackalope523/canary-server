namespace Repository
{
    public class ConversationLink : Entity
    {
        public enum ConversationLinkType { Member, Admin }

        public long? UserId { get; set; }
        public long ConversationId { get; set; }
        public ConversationLinkType Type { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Conversation? Conversation { get; set; }
    }
}