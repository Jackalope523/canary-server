
namespace Repository
{
    internal class PostLinkFactory
    {
        internal SnapshotLink Create(User user, Snapshot snapshot)
        {
            return new SnapshotLink
            {
                UserId = user.Id,
                PostId = snapshot.Id,
                Type = SnapshotLink.SnapshotLinkType.RateUp,
                Time = DateTimeOffset.MinValue
            };
        }
    }
}
