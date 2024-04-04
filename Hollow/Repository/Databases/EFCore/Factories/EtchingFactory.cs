
namespace Repository
{
    internal class EtchingFactory
    {
        private int created = 0;
        
        internal Post Create(User owner, Event location)
        {
            created++;
            return new Post
            {
                OwnerId = owner.Id,
                EventId = location.Id,
                PostedAt = DateTime.MinValue,
                PhotoURL = "URL " + created,
                IsHidden = false
            };
        }
        internal Post Create(User owner, Event location, DateTimeOffset postedAt)
        {
            created++;
            return new Post
            {
                OwnerId = owner.Id,
                EventId = location.Id,
                PostedAt = postedAt,
                PhotoURL = "URL " + created,
                IsHidden = false
            };
        }
    }
}
