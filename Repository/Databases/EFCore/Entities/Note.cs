
namespace Repository.Entities
{
    internal class Note
    {
        internal ulong Id { get; set; }
       
        internal ulong NotifierId { get; set; }
        internal User Notifier { get; set; }
        internal ulong RecipientId { get; set; }
        internal User Recipient { get; set; }
        internal DateTimeOffset Time { get; set; }
        internal string Message { get; set; }
        internal string Action { get; set; }
        internal bool Read { get; set; } 
    }
}
