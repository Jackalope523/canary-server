namespace Repository
{
    public class MessageLink : Entity
    {
        public enum MessageLinkType { Seen }

        public long? UserId { get; set; }
        public long MessageId { get; set; }
        public MessageLinkType Type { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Message? Message { get; set; }
    }
}
