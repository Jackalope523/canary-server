
namespace Repository.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }
        internal User Owner { get; set; }
        public Guid EventId { get; set; }
        internal Event Event { get; set; }
        public DateTimeOffset PostedAt { get; init; }
        public string PhotoURL { get; set; }
    }
}
