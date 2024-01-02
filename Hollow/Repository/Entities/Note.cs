
namespace Repository.Entities
{
    public class Note
    {
        public ulong Id { get; set; }
       
        public ulong NotifierId { get; set; }
        public User Notifier { get; set; }
        public ulong RecipientId { get; set; }
        public User Recipient { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }
        public bool Read { get; set; } 
    }
}
