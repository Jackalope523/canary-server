using Repository.Entities;

namespace Repository
{
    public class DeletePriorityComparer : IComparer<Entity>
    {
        private static int GetPriority(Entity entity)
        {
            if (entity is User || entity is Banner || entity is RumoredGathering)
            {
                return 4;
            }
            else if (
                entity is Gathering || 
                entity is Feedback || 
                entity is Subscription || 
                entity is Penalty || 
                entity is Telegram ||
                entity is BannerLink ||
                entity is Investigation ||
                entity is Rumor
                )
            {
                return 3;
            }
            else if (
                entity is Snapshot || 
                entity is UserRelationship ||
                entity is GatheringLink ||
                entity is GatheringReport || 
                entity is UserReport ||
                entity is GuestClearance ||
                entity is RumorReport
                ) 
            {
                return 2;
            }
            else if (entity is SnapshotLink || entity is SnapshotReport)
            {
                return 1;
            }
            else
            {
                throw new ArgumentException($"Object of type {entity.GetType().ToString()} can not be assigned a delete priority.");
            }
        }

        public int Compare(Entity x, Entity y)
        {
            return GetPriority(x) - GetPriority(y);
        }
    }

}
