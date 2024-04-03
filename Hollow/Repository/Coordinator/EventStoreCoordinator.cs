using Core.Boundaries;
using Shared;

namespace Repository
{
    public class EventStoreCoordinator : IEventDatabase
    {
        private readonly IEventDatabase store;

        public EventStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFCoreEventStore(flag);
        }

        public async Task<EventShard> CreateEventAsync(ulong hostId, string name, string description, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum, Character character, double radius, bool isDynamic)
        {
            return await store.CreateEventAsync(hostId, name, description, startTime, latitude, longitude, groupMinimum, groupMaximum, character, radius, isDynamic);
        }

        public async Task DeleteEventAsync(ulong eventId)
        {
            await store.DeleteEventAsync(eventId);  
        }

        public async Task<EventShard> FindCurrentEventForUserAsync(ulong id) 
        {
            return await store.FindCurrentEventForUserAsync(id);
        }

        public async Task<List<EventShard>> FindUpcomingEventsForUserAsync(ulong id) 
        {
            return await store.FindUpcomingEventsForUserAsync(id);
        }

        public async Task<List<EventShard>> FindPastEventsForUserAsync(ulong id)
        {
            return await store.FindPastEventsForUserAsync(id);
        }

        public async Task<EventShard> FindEventAsync(ulong id)
        {
            return await store.FindEventAsync(id);
        }

        public async Task<List<EventShard>> FindEventsAsync(double latitude, double longitude, double distance)
        {
            return await store.FindEventsAsync(latitude, longitude, distance);
        }       
        
        public async Task RemoveUserAsync(ulong userId, ulong eventId) 
        { 
            await store.RemoveUserAsync(userId, eventId);
        }

        public async Task UpdateEventAsync(ulong id, List<(string Property, object Value)> edits)
        {
            await store.UpdateEventAsync(id, edits); 
        }   

        public async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetGuestHistoryAsync(ulong id)
        {
            return await store.GetGuestHistoryAsync(id);
        }   
        
        public async Task<List<EventShard>> FindEventsByUserAsync(ulong userId)
        {
            return await store.FindEventsByUserAsync(userId);
        }  
        
        public async Task<EventBond?> GetUserStateAsync(ulong userId, ulong eventId)
        {
            return await store.GetUserStateAsync(userId, eventId);
        }

        public async Task SetUserStateAsync(ulong userId, ulong eventId, EventBond userState, DateTimeOffset time)
        {
            await store.GetUserStateAsync(userId, eventId);
        }

        public async Task<List<(UserSilhouette User, EventBond State)>> GetAllUsersAsync(ulong eventId)
        {
            return await store.GetAllUsersAsync(eventId);
        }

        public async Task EndEventAsync(ulong id, DateTimeOffset time)
        {
            await store.EndEventAsync(id, time);
        }
    }
}

