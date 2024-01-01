
namespace Repository.Entities
{
    public class Notification
    {
        public ulong Id { get; set; }
        public ulong RecipientId { get; set; }
        public User Recipient { get; set; }
        public string Notes { get; set; }
    }
}
