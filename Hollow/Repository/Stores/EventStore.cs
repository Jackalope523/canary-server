using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Shared;

namespace Repository
{
    public class EventStore : QueryStore, IEventDatabase
    {
        public static IEventDatabase EventDatabaseAccess => new EventStore(new TestSentry());

        public EventStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<EventShard> CreateEventAsync(ulong hostId, string name, string description, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum, Character character, double radius, bool isDynamic)
        {
            Event toCreate = new()
            {
                HostId = hostId,
                Name = name,
                Description = description,
                StartTime = startTime,
                Location = new Point(longitude, latitude),
                GroupMinimum = groupMinimum,
                GroupMaximum = groupMaximum,
                Radius = radius,
                IsDynamic = isDynamic,
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,

            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Events.Add(toCreate));
            UserSilhouette host = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == hostId).Select(u => new UserSilhouette(u.Id, u.Name)).SingleAsync());


            return new EventShard
                (
                   toCreate.Id,
                   host,
                   toCreate.Name,
                   toCreate.Description,
                   toCreate.StartTime,
                   toCreate.Location.Y,
                   toCreate.Location.X,
                   toCreate.EndTime,
                   toCreate.State,
                   toCreate.GroupMinimum,
                   toCreate.GroupMaximum,
                   new Character(
                   toCreate.Extroversion,
                   toCreate.Athleticisme,
                   toCreate.Chaos,
                   toCreate.Competitiveness,
                   toCreate.Industriousness,
                   toCreate.NightOwl,
                   toCreate.Openness),
                   toCreate.Radius,
                   toCreate.IsDynamic                   
                   );
        }
        public async Task<EventShard> FindCurrentEventForUserAsync(ulong id) 
        {
            ulong currentEventId = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.EventLinks.
            Where(l => l.SelfId == id && l.Type == EventUserState.Present).
            Select(l => l.OtherId).
            SingleAsync());

            EventShard @event = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Events.
            Where(e => e.Id == currentEventId).
            Select(e => new EventShard
               (
                   e.Id,
                   new UserSilhouette(e.HostId, null),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.State,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new Character(
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness),
                   e.Radius,
                   e.IsDynamic
                   )).SingleAsync());

            UserSilhouette host = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Users.
            Where(u => u.Id == @event.Host.Id).
            Select(u => new UserSilhouette(u.Id, u.Name)).
            SingleAsync());

            return @event with { Host = host };
        }
        public async Task<List<EventShard>> FindUpcomingEventsForUserAsync(ulong id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.SelfId == id && l.Type == EventUserState.Guest).
            Join(
                ctx.Events,
                l => l.OtherId,
                e => e.Id,
                (l,e) => new EventShard
                (
                   e.Id,
                   new UserSilhouette(e.HostId, e.Host.Name),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.State,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new Character(
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness),
                   e.Radius,
                   e.IsDynamic
                )).
            ToListAsync());
        }
        public async Task<List<EventShard>> FindPastEventsForUserAsync(ulong id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
           ctx.EventLinks.
           Where(l => l.SelfId == id && l.Type == EventUserState.Left).
           Join(
               ctx.Events,
               l => l.OtherId,
               e => e.Id,
               (l, e) => new EventShard
               (
                  e.Id,
                  new UserSilhouette(e.HostId, e.Host.Name),
                  e.Name,
                  e.Description,
                  e.StartTime,
                  e.Location.Y,
                  e.Location.X,
                  e.EndTime,
                  e.State,
                  e.GroupMinimum,
                  e.GroupMaximum,
                  new Character(
                  e.Extroversion,
                  e.Athleticisme,
                  e.Chaos,
                  e.Competitiveness,
                  e.Industriousness,
                  e.NightOwl,
                  e.Openness),
                  e.Radius,
                  e.IsDynamic
               )).
           ToListAsync());
        }
        public async Task<EventShard> FindEventAsync(ulong id)
        {
            EventShard @event = await storeSentry.ExecuteReadAsync(ctx => ctx.Events.Where(e => e.Id == id).Select(e => new EventShard
               (
                   e.Id,
                   new UserSilhouette(e.HostId, null),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.State,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new Character(
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness),
                   e.Radius,
                   e.IsDynamic
               )).SingleAsync());

            UserSilhouette host = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == @event.Host.Id).Select(u => new UserSilhouette(u.Id, u.Name)).SingleAsync()) ;

            return @event with {Host = host } ;
        }
        public async Task<List<EventThinSlice>> FindEventsAsync(double latitude, double longitude, double distance)
        {
            return await storeSentry.ExecuteReadAsync(ctx => ctx.Events.Where(e => e.Location.Distance(new Point(longitude, latitude)) <= distance && !e.EndTime.HasValue).
                                Join(ctx.Users, 
                                e => e.HostId, 
                                u => u.Id, 
                                (e,u) => new EventThinSlice(e.Id, new UserSilhouette(u.Id, u.Name), e.Location.Y, e.Location.X)).
                                ToListAsync());
        }     
        public async Task<List<UserSilhouette>> GetGuestListAsync(ulong id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.OtherId == id && l.Type == EventUserState.Guest).
            Join(
                ctx.Users,
                l => l.SelfId,
                u => u.Id,
                (_,u) => new UserSilhouette(u.Id, u.Name)
                ).
            ToListAsync());
        }    
        public async Task<bool> RemoveUserAsync(ulong userId, ulong eventId) 
        { 
            await storeSentry.ExecuteWriteAsync(ctx => 
            ctx.EventLinks.
            Where(l => l.SelfId == userId && l.OtherId == eventId).
            ExecuteDeleteAsync());

            return true;
        }
        public async Task<bool> UpdateEventAsync(ulong id, List<(string Property, object Value)> edits)
        {
            Event e = new() { Id = id };
            storeSentry.DiscussWrite(ctx => ctx.Events.Attach(e));

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case "Description":
                        e.Description = (string)Value;
                        break;
                    case "State":
                        e.State = (EventState)Value;
                        break;
                    case "EndTime":
                        e.EndTime = (DateTimeOffset?)Value;
                        break;
                    default:
                        throw new InvalidInputException("Property named \"" + Property + "\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(Property).IsModified = true);
            }
            await storeSentry.ExecuteWriteAsync();
            return true;
        }   
        public async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetGuestHistoryAsync(ulong id)
        {
            var arrivalTimes = storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.OtherId == id && l.Type == EventUserState.Present).
            Join(
                ctx.Users,
                l => l.SelfId,
                u => u.Id,
                (l, u) => new { l.Time, u.Id, u.Name }
                ).
            ToListAsync());
   
            var departureTimes = storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.OtherId == id && l.Type == EventUserState.Left).
            Join(
                ctx.Users,
                l => l.SelfId,
                u => u.Id,
                (l, u) => new { l.Time, u.Id }
                ).
            ToListAsync());

            Dictionary<ulong, DateTimeOffset> organizedDepartureTimes = new();
            foreach (var v in await departureTimes)
            {
                organizedDepartureTimes.Add(v.Id, v.Time);
            }

            List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> toReturn = new();
            foreach (var v in await arrivalTimes)
            {
                DateTimeOffset? departureTime;
                try
                {
                    departureTime = organizedDepartureTimes[v.Id];
                }
                catch (KeyNotFoundException)
                {
                    departureTime = null;
                }

                toReturn.Add((v.Time, departureTime, new UserSilhouette(v.Id, v.Name)));
            }

            return toReturn;
        }      
        public async Task<List<EventShard>> FindEventsByUserAsync(ulong userId)
        {
           return await storeSentry.ExecuteReadAsync(ctx =>
           ctx.Events.
           Where(e => e.HostId == userId).
           Join(ctx.Users,
           e => e.HostId,
           u => u.Id,
           (e, u) => new EventShard(
               e.Id, 
               new UserSilhouette(u.Id, u.Name), 
               e.Name, 
               e.Description, 
               e.StartTime, 
               e.Location.Y, 
               e.Location.X, 
               e.EndTime, 
               e.State, 
               e.GroupMinimum, 
               e.GroupMaximum, 
               new Character(
                   e.Extroversion,
                   e.Athleticisme, 
                   e.Chaos, 
                   e.Competitiveness,
                   e.Industriousness, 
                   e.NightOwl,
                   e.Openness
                   ), 
               e.Radius, 
               e.IsDynamic)).
           ToListAsync());
        }      
        public async Task<EventUserState?> GetUserStateAsync(ulong userId, ulong eventId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.SelfId == userId && l.OtherId == eventId).
            Select(l => l.Type).
            SingleAsync());
        }
        public async Task<bool> SetUserStateAsync(ulong userId, ulong eventId, EventUserState userState)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
                ctx.EventLinks.
                Add(new EventLink { SelfId = userId, OtherId = eventId, Type = userState }
                ));

            return true;           
        }
        public async Task<List<(UserSilhouette User, EventUserState State)>> GetAllUsersAsync(ulong eventId)
        {
            var users = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.OtherId == eventId).
            Join(
                ctx.Users,
                l => l.SelfId,
                u => u.Id,
                (l, u) => new { u.Id, u.Name, l.Type }
                ).
            ToListAsync());

            List<(UserSilhouette User, EventUserState State)> toReturn = new();
            for (int i = 0; i < users.Count; i++)
            {
                toReturn.Add((new UserSilhouette(users[i].Id, users[i].Name), users[i].Type));
            }
            return toReturn;
        }
        public async Task<bool> EndEventAsync(ulong id)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.Type == EventUserState.Present).
            ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Type, EventUserState.Left)));

            await storeSentry.ExecuteWriteAsync(ctx =>
            ctx.EventLinks.
            Where(l =>
            l.Type == EventUserState.Watching &&
            l.Type == EventUserState.Guest &&
            l.Type == EventUserState.Incoming).
            ExecuteDeleteAsync()
            );

            return true;
        }
    }
}

