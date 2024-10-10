
namespace Repository.Entities
{
    public class Telegram
    {
        public ulong Id { get; set; }
        public ulong NotifierId { get; set; }
        public ulong RecipientId { get; set; }
        public DateTimeOffset Time { get; set; }
        public TelegramMessage Message { get; set; }
        public string Action { get; set; }
        public bool Read { get; set; }

        // Navigation Properties
        internal User? Notifier { get; set; }
        internal User? Recipient { get; set; }
    }
}
