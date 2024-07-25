
namespace Repository
{
    internal class SnapshotFactory
    {
        private int created = 0;
        
        internal Snapshot Create(User owner, Gathering location)
        {
            created++;
            return new Snapshot
            {
                OwnerId = owner.Id,
                GatheringId = location.Id,
                PostedAt = DateTime.MinValue,
            };
        }
        internal Snapshot Create(User owner, Gathering location, DateTimeOffset postedAt)
        {
            created++;
            return new Snapshot
            {
                OwnerId = owner.Id,
                GatheringId = location.Id,
                PostedAt = postedAt,
            };
        }
    }
}
