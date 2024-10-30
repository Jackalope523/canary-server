using Repository.Entities;

namespace Repository
{
    internal interface IFactoryObserver
    {
        internal void Notify(User entity);
        internal void Notify(Gathering entity);
        internal void Notify(UserRelationship entity);
        internal void Notify(GatheringLink entity);
        internal void Notify(SnapshotLink entity);
        internal void Notify(UserReport entity);
        internal void Notify(GatheringReport entity);
        internal void Notify(SnapshotReport entity);
        internal void Notify(Snapshot entity);
        internal void Notify(Telegram entity);
        internal void Notify(Subscription entity);
        internal void Notify(Penalty entity);
        internal void Notify(Banner entity);
        internal void Notify(BannerLink entity);
        internal void Notify(GuestClearance entity);
        internal void Notify(Feedback entity);
    }
}
