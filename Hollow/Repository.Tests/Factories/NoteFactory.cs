
namespace Repository
{
    internal class NoteFactory
    {
        private int produced = 0;

        public Entities.Note Create(User Notifier, User Recipient)
        {
            produced++;
            return new Entities.Note
            {
                NotifierId = Notifier.Id,
                RecipientId = Recipient.Id,
                Time = DateTimeOffset.MinValue,
                Message = "Message " + produced,
                Action = "Action " + produced,
                Read = false
            };
        }
    }
}
