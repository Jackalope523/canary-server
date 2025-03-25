using Repository.Databases.EFCore.Entities.Messages;

namespace Repository
{
    public class Conversation : Entity
    {
        public string Title { get; set; } = DefaultTitle;


        // Navigation Properties
        public List<ConversationLink>? ConversationLinks { get; set; }
        public List<Message>? Messages { get; set; }

        // Default Values
        public static string DefaultTitle { get; set; } = "";

    }
}
