using Core.Boundaries;


namespace Repository
{
    public class GatheringStoreCoordinator : IGatheringDatabase
    {
        private readonly IGatheringDatabase store;

        public GatheringStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreGatheringStore(flag);
        }

        public async Task<CoreGathering> CreateGatheringAsync(ulong hostId, string name, string description, DateTimeOffset startTime, double latitude, double longitude, string friendlyLocation, int groupMinimum, int groupMaximum, CharacterShard character, double Radius, bool isDynamic, int degreeOfPrivacy)
        {
            return await store.CreateGatheringAsync(hostId, name, description, startTime, latitude, longitude, friendlyLocation, groupMinimum, groupMaximum, character, Radius, isDynamic, degreeOfPrivacy);
        }

        public async Task DeleteGatheringAsync(ulong gatheringId)
        {
            await store.DeleteGatheringAsync(gatheringId);  
        }

        public async Task<CoreGathering> FindCurrentGatheringForUserAsync(ulong id) 
        {
            return await store.FindCurrentGatheringForUserAsync(id);
        }

        public async Task<List<CoreGathering>> FindUpcomingGatheringsForUserAsync(ulong id) 
        {
            return await store.FindUpcomingGatheringsForUserAsync(id);
        }

        public async Task<List<CoreGathering>> FindSurveyingGatheringsForUserAsync(ulong id) 
        {
            return await store.FindSurveyingGatheringsForUserAsync(id);
        }

        public async Task<List<CoreGathering>> FindPastGatheringsForUserAsync(ulong id)
        {
            return await store.FindPastGatheringsForUserAsync(id);
        }

        public async Task<CoreGathering> FindGatheringAsync(ulong id)
        {
            return await store.FindGatheringAsync(id);
        }

        public async Task<List<CoreGathering>> FindGatheringsAsync(double latitude, double longitude, double distance)
        {
            return await store.FindGatheringsAsync(latitude, longitude, distance);
        }       
        
        public async Task DeleteUserStateAsync(ulong userId, ulong gatheringId) 
        { 
            await store.DeleteUserStateAsync(userId, gatheringId);
        }

        public async Task UpdateGatheringAsync(ulong id, List<(string Property, object Value)> edits)
        {
            await store.UpdateGatheringAsync(id, edits); 
        }   

        public async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserShard User)>> GetGuestHistoryAsync(ulong id)
        {
            return await store.GetGuestHistoryAsync(id);
        }   
        
        public async Task<List<CoreGathering>> FindGatheringsByUserAsync(ulong userId)
        {
            return await store.FindGatheringsByUserAsync(userId);
        }  
        
        public async Task<GatheringBond?> GetUserStateAsync(ulong userId, ulong gatheringId)
        {
            return await store.GetUserStateAsync(userId, gatheringId);
        }

        public async Task SetUserStateAsync(ulong userId, ulong gatheringId, GatheringBond userState, DateTimeOffset time)
        {
            await store.SetUserStateAsync(userId, gatheringId, userState, time);
        }

        public async Task<List<(UserShard User, GatheringBond State)>> GetAllUsersAsync(ulong gatheringId)
        {
            return await store.GetAllUsersAsync(gatheringId);
        }

        public async Task TerminateGatheringAsync(ulong id, DateTimeOffset time)
        {
            await store.TerminateGatheringAsync(id, time);
        }

        public async Task<bool> CanJoin(ulong gatheringId, ulong userId)
        {
            return await store.CanJoin(gatheringId, userId);
        }

        public async Task<List<ulong>> GetAuthorizedGuests(ulong gatheringId)
        {
            return await store.GetAuthorizedGuests(gatheringId);
        }
    }
}

