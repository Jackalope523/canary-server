using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class SnapshotStoreCoordinator : ISnapshotDatabase
    {
        private readonly ISnapshotDatabase store;

        public SnapshotStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreSnapshotStore(flag);
        }

        public async Task<SnapshotShard> AddSnapshotAsync(ulong gatheringId, ulong posterId, DateTimeOffset timePosted)
        { 
             return await store.AddSnapshotAsync(gatheringId, posterId, timePosted);  
        }

        public async Task<List<SnapshotShard>> GenerateColumnForUserAsync(ulong id, DateTimeOffset depthCharge, DateTimeOffset lastDepthCharge)
        {
           return await store.GenerateColumnForUserAsync(id, depthCharge, lastDepthCharge);   
        }

        public async Task<SnapshotShard> GetSnapshotAsync(ulong id)
        {
            return await store.GetSnapshotAsync(id);
        }

        public async Task<List<SnapshotShard>> GetSnapshotsByUserAsync(ulong id)
        {
            return await store.GetSnapshotsByUserAsync(id);
        }

        public async Task AcclaimSnapshotAsync(ulong postId, ulong voterId)
        {           
          await store.AcclaimSnapshotAsync(postId, voterId);
        }

        public async Task DeleteSnapshotAsync(ulong postId)
        {
            await store.DeleteSnapshotAsync(postId);
        }

        public async Task DeleteSnapshotAcclaimAsync(ulong postId, ulong voterId)
        {
            await store.DeleteSnapshotAcclaimAsync(postId, voterId);
        }

        public async Task<List<SnapshotShard>> GetSnapshotsForGatheringAsync(ulong id)
        {
            return await store.GetSnapshotsForGatheringAsync(id);
        }
    }
}
