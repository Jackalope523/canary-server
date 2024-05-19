
namespace Repository
{
    internal class SnapshotFactory
    {
        private int created = 0;
        
        internal Post Create(User owner, Gathering location)
        {
            created++;
            return new Post
            {
                OwnerId = owner.Id,
                GatheringId = location.Id,
                PostedAt = DateTime.MinValue,
                PhotoURL = "URL " + created,
                IsHidden = false
            };
        }
        internal Post Create(User owner, Gathering location, DateTimeOffset postedAt)
        {
            created++;
            return new Post
            {
                OwnerId = owner.Id,
                GatheringId = location.Id,
                PostedAt = postedAt,
                PhotoURL = "URL " + created,
                IsHidden = false
            };
        }
    }
}
