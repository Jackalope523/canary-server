using Core.Boundaries;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EtchingStoreCoordinator : IEtchingDatabase
    {
        private readonly IEtchingDatabase store;

        public EtchingStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreEtchingStore(flag);
        }

        public async Task<EtchingShard> AddEtchingAsync(ulong eventId, ulong posterId, DateTimeOffset timePosted)
        { 
             return await store.AddEtchingAsync(eventId, posterId, timePosted);  
        }

        public async Task<List<EtchingShard>> GenerateFeedForUserAsync(ulong id, DateTimeOffset depthCharge, DateTimeOffset lastDepthCharge)
        {
           return await store.GenerateFeedForUserAsync(id, depthCharge, lastDepthCharge);   
        }

        public async Task<EtchingShard> GetEtchingAsync(ulong id)
        {
            return await store.GetEtchingAsync(id);
        }

        public async Task<List<EtchingShard>> GetEtchingsByUserAsync(ulong id)
        {
            return await store.GetEtchingsByUserAsync(id);
        }

        public async Task RateEtchingAsync(ulong postId, ulong voterId, UserRating rating)
        {           
          await store.RateEtchingAsync(postId, voterId, rating);
        }

        public async Task RemoveEtchingAsync(ulong postId)
        {
            await store.RemoveEtchingAsync(postId);
        }

        public async Task RemoveEtchingRatingAsync(ulong postId, ulong voterId)
        {
            await store.RemoveEtchingRatingAsync(postId, voterId);
        }

        public async Task<List<EtchingShard>> GetEtchingsForEventAsync(ulong id)
        {
            return await store.GetEtchingsForEventAsync(id);
        }

        public async Task HideEtchingAsync(ulong etchingId)
        {
            await store.HideEtchingAsync(etchingId);
        }
    }
}
