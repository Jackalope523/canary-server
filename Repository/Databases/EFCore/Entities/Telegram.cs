
namespace Repository.Entities
{
    public class Telegram
    {
        public ulong Id { get; set; }
       
        public ulong NotifierId { get; set; }
        internal User Notifier { get; set; }
        public ulong RecipientId { get; set; }
        internal User Recipient { get; set; }
        public DateTimeOffset Time { get; set; }
        public TelegramMessage Message { get; set; }
        public string Action { get; set; }
        public bool Read { get; set; } 
    }
}
