namespace Repository
{
    public class ConversationLink : Entity
    {
        public long UserId { get; set; }
        public long ConversationId { get; set; }
        public DateTimeOffset LastSeen { get; set; }
        public MembershipType Type { get; set; }
        public bool Muted { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Conversation? Conversation { get; set; }
    }
}