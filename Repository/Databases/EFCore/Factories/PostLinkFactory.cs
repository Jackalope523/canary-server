
namespace Repository
{
    internal class PostLinkFactory
    {
        internal SnapshotLink Create(User user, Snapshot snapshot)
        {
            return new SnapshotLink
            {
                UserId = user.Id,
                SnapshotId = snapshot.Id,
                Type = SnapshotLink.SnapshotLinkType.Appreciate,
                Time = DateTimeOffset.MinValue
            };
        }
    }
}
