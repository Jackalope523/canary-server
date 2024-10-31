
namespace Repository
{
    internal class NoteFactory : Factory
    {
        private int produced = 0;

        internal Entities.Telegram Create(User Notifier, User Recipient)
        {
            produced++;
            return new Entities.Telegram
            {
                NotifierId = Notifier.Id,
                RecipientId = Recipient.Id,
                Time = DateTimeOffset.MinValue,
                Message = TelegramMessage.UserAppreciated,
                Action = "Action " + produced,
                Read = false
            };
        }
    }
}
