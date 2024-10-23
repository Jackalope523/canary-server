namespace Repository.Entities
{
    public class Telegram
    {
        public long Id { get; set; } = DefaultId;
        public bool SoftDeleted { get; set; } = DefaultSoftDeleted;
        public long NotifierId { get; set; }
        public long RecipientId { get; set; }
        public DateTimeOffset Time { get; set; }
        public TelegramMessage Message { get; set; }
        public string Action { get; set; }
        public bool Read { get; set; }

        // Navigation Properties
        internal User? Notifier { get; set; }
        internal User? Recipient { get; set; }

        // Default Values
        public static long DefaultId { get; set; } = 0;
        public static bool DefaultSoftDeleted { get; set; } = false;
    }
}
