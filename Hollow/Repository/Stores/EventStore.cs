using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Shared;

namespace Repository
{
    public class EventStore : QueryStore, IEventDatabase
    {
        public EventStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<EventShard> CreateEventAsync(ulong hostId, string name, string description, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum, Character character, double radius, bool isDynamic)
        {
            Event toCreate = new()
            {
                Name = name,
                Description = description,
                StartTime = startTime,
                HostId = hostId,
                Location = new CoordinateFactory().Create(longitude, latitude),
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

            EventLink hostLink = new() 
            { 
                UserId = hostId,
                EventId = toCreate.Id,
                Type = EventBond.Guest,      
                Time = DateTimeOffset.UtcNow
            };
            
            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventLinks.Add(hostLink));

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
                   toCreate.IsDynamic,
                   toCreate.IsPendingDeletion
                   );
        }
        public async Task DeleteEventAsync(ulong eventId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => 
                ctx.Events.
                Where(e => e.Id == eventId).
                ExecuteUpdate(setter => setter.SetProperty(e => e.IsPendingDeletion, true)));       
        }
        public async Task<EventShard> FindCurrentEventForUserAsync(ulong id) 
        {
            ulong? currentEvent = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Users.
            Where(u => u.Id == id).
            Select(u => u.CurrentEvent).
            SingleAsync());

            EventShard? @event = null;
            if (currentEvent != null)
            {
                @event = await storeSentry.ExecuteReadAsync(ctx =>
                    ctx.Events.
                    Where(e => e.Id == currentEvent).
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
                        e.IsDynamic,
                        e.IsPendingDeletion
                        )).SingleAsync());

                UserSilhouette host = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == @event.Host.Id).
                Select(u => new UserSilhouette(u.Id, u.Name)).
                SingleAsync());


                @event =  @event with { Host = host };
            }
            return @event;          
        }
        public async Task<List<EventShard>> FindUpcomingEventsForUserAsync(ulong id) 
        {
             List<EventShard> upcomingEvents = await storeSentry.ExecuteReadAsync(ctx =>
             ctx.EventLinks.
             Where(l => l.UserId == id && l.Type == EventBond.Guest).
             Join(
                ctx.Events.Where(e => e.State == EventState.Upcoming || e.State == EventState.Open),
                l => l.EventId,
                e => e.Id,
                (l, e) => new
                {
                    e.Id,
                    e.Name,
                    e.HostId,
                    e.Description,
                    e.StartTime,
                    e.Location,
                    e.EndTime,
                    e.State,
                    e.GroupMinimum,
                    e.GroupMaximum,
                    e.Extroversion,
                    e.Athleticisme,
                    e.Chaos,
                    e.Competitiveness,
                    e.Industriousness,
                    e.NightOwl,
                    e.Openness,
                    e.Radius,
                    e.IsDynamic,
                    e.IsPendingDeletion
                }).
             Join(
                 ctx.Users,
                 e => e.HostId,
                 u => u.Id,
                 (e, u) => new EventShard
                 (
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
                    e.Openness),
                    e.Radius,
                    e.IsDynamic,
                    e.IsPendingDeletion
                 )).
               ToListAsync());

            List<ulong> toExclude = await storeSentry.ExecuteReadAsync(ctx =>
               ctx.EventLinks.
               Where(l => l.UserId == id && l.Type == EventBond.Arrived).
               Select(l => l.EventId). 
               ToListAsync());

            for (int i = 0; i < upcomingEvents.Count; i++)
            {
                if (toExclude.Contains(upcomingEvents[i].Id))
                {
                    upcomingEvents.RemoveAt(i);
                    i--;
                }
            }

            return upcomingEvents;
        }
        public async Task<List<EventShard>> FindPastEventsForUserAsync(ulong id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.UserId == id && l.Type == EventBond.Left).
            Join(
               ctx.Events.Where(e => e.State == EventState.Ended),
               l => l.EventId,
               e => e.Id,
               (l, e) => new
               {
                  e.Id,               
                  e.Name,
                  e.HostId,
                  e.Description,
                  e.StartTime,
                  e.Location,
                  e.EndTime,
                  e.State,
                  e.GroupMinimum,
                  e.GroupMaximum,                 
                  e.Extroversion,
                  e.Athleticisme,
                  e.Chaos,
                  e.Competitiveness,
                  e.Industriousness,
                  e.NightOwl,
                  e.Openness,
                  e.Radius,
                  e.IsDynamic,
                  e.IsPendingDeletion
               }).
            Join(
                ctx.Users,
                e => e.HostId,
                u => u.Id,
                (e, u) => new EventShard
                (
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
                   e.Openness),
                   e.Radius,
                   e.IsDynamic,
                   e.IsPendingDeletion
                )).
              ToListAsync());
        }
        public async Task<EventShard> FindEventAsync(ulong id)
        {
            EventShard @event = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.Events.
            Where(e => e.Id == id).
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
                   e.IsDynamic,
                   e.IsPendingDeletion
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
            Where(l => l.EventId == id && l.Type == EventBond.Guest).
            Join(
                ctx.Users,
                l => l.UserId,
                u => u.Id,
                (_,u) => new UserSilhouette(u.Id, u.Name)
                ).
            ToListAsync());
        }    
        public async Task RemoveUserAsync(ulong userId, ulong eventId) 
        { 
            await storeSentry.ExecuteWriteAsync(ctx => 
            ctx.EventLinks.
            Where(l => l.UserId == userId && l.EventId == eventId).
            ExecuteDeleteAsync());
        }
        public async Task UpdateEventAsync(ulong id, List<(string Property, object Value)> edits)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Event e = new() { Id = id };
            storeSentry.DiscussWrite(ctx => ctx.Events.Attach(e), currentDiscussion);

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case nameof(EventShard.Description):
                        e.Description = (string)Value;
                        break;
                    case nameof(EventShard.State):
                        e.State = (EventState)Value;
                        break;
                    case nameof(EventShard.StartTime):
                        e.StartTime = (DateTimeOffset)Value;
                        break;
                    case "Location":
                        var Location = ((double,double))Value;
                        e.Location = new CoordinateFactory().Create(Location.Item2, Location.Item1);
                        break;              
                    case nameof(EventShard.Radius):
                        e.Radius = (double)Value;
                        break;
                    case nameof(EventShard.IsDynamic):
                        e.IsDynamic = (bool)Value;
                        break;
                    case nameof(EventShard.GroupMinimum):
                        e.GroupMinimum = (int)Value;                    
                        break;
                    case nameof(EventShard.GroupMaximum):
                        e.GroupMaximum = (int)Value;
                        break;
                    default:
                        throw new InvalidInputException("Property named \"" + Property + "\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(Property).IsModified = true, currentDiscussion);
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }   
        public async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)>> GetGuestHistoryAsync(ulong id)
        {
            var times = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.EventId == id && (l.Type == EventBond.Arrived || l.Type == EventBond.Left)).
            Join(
                ctx.Users,
                l => l.UserId,
                u => u.Id,
                (l,u) => new { u.Id, u.Name, l.Time, l.Type }
                ).
            ToListAsync());

            Dictionary<ulong,List<(string, DateTimeOffset, EventBond)>> history = new();
            foreach (var item in times)
            {
                if (!history.ContainsKey(item.Id)) history.Add(item.Id, new());
                history[item.Id].Add((item.Name, item.Time, item.Type));               
            }

            List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> toReturn = new();
            ulong userId;
            string userName;
            DateTimeOffset arrivalTime;
            DateTimeOffset? departureTime;
            foreach ( var entry in history)
            {
                entry.Value.Sort((x,y) => DateTimeOffset.Compare(x.Item2, y.Item2));

                (string firstName, DateTimeOffset firstTime, _) = entry.Value.First();
                (_, DateTimeOffset lastTime, EventBond lastState) = entry.Value.Last();

                userId = entry.Key;
                userName = firstName;
                arrivalTime = firstTime;
                if (lastState == EventBond.Left) departureTime = lastTime;
                else departureTime = null;
                
                toReturn.Add((arrivalTime, departureTime, new UserSilhouette(userId, userName)));
            }

            return toReturn;
        }      
        public async Task<List<EventShard>> FindEventsByUserAsync(ulong userId)
        {
           return await storeSentry.ExecuteReadAsync(ctx =>
           ctx.Events.
           Where(e => e.HostId == userId).
           Join(
               ctx.Users,
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
                    e.IsDynamic,
                    e.IsPendingDeletion
                    )).
           ToListAsync());
        }      
        public async Task<EventBond?> GetUserStateAsync(ulong userId, ulong eventId)
        {
            var states = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.UserId == userId && l.EventId == eventId).
            Select(l => new { l.Type, l.Time }).
            ToListAsync());

            states.Sort((x,y) => DateTimeOffset.Compare(x.Time, y.Time));

            return states.Last().Type;
        }
        public async Task SetUserStateAsync(ulong userId, ulong eventId, EventBond userState)
        {
            EventLink toAdd = new() 
            { 
                UserId = userId, 
                EventId = eventId, 
                Type = userState 
            };

            ulong id = await storeSentry.ExecuteReadAsync(ctx => 
                       ctx.EventLinks.
                       Where(l => l.UserId == userId && l.EventId == eventId && l.Type == userState).
                       Select(l => l.Id).
                       SingleOrDefaultAsync());

            if (id != 0)
            {
                toAdd.Id = id;
            }

            await storeSentry.ExecuteWriteAsync(ctx => ctx.EventLinks.Update(toAdd));

            if (userState == EventBond.Left || userState == EventBond.Kicked || userState == EventBond.Arrived)
            {
                Discussion currentDiscussion = storeSentry.BeginDiscussion();

                User u;
                if (userState == EventBond.Left || userState == EventBond.Kicked)
                {
                    u = new() { Id = userId, CurrentEvent = null };
                }
                else
                {
                    u = new() { Id = userId, CurrentEvent = eventId };
                }

                storeSentry.DiscussWrite(ctx => ctx.Users.Attach(u), currentDiscussion);
                storeSentry.DiscussWrite(ctx => ctx.Entry(u).Property(nameof(u.CurrentEvent)).IsModified = true, currentDiscussion);
                await storeSentry.EndDiscussionAsync(currentDiscussion);
            } 
        }
        public async Task<List<(UserSilhouette User, EventBond State)>> GetAllUsersAsync(ulong eventId)
        {
            var users = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.EventLinks.
            Where(l => l.EventId == eventId).
            Join(
                ctx.Users,
                l => l.UserId,
                u => u.Id,
                (l, u) => new { u.Id, u.Name, l.Time, l.Type }
                ).
            ToListAsync());

            Dictionary<ulong, List<(string, DateTimeOffset, EventBond)>> history = new();
            foreach (var item in users)
            {
                if (!history.ContainsKey(item.Id)) history.Add(item.Id, new());
                history[item.Id].Add((item.Name, item.Time, item.Type));
            }

            List<(UserSilhouette User, EventBond State)> toReturn = new();
            ulong userId;
            string userName;
            EventBond userState;
            foreach (var entry in history)
            {
                entry.Value.Sort((x, y) => DateTimeOffset.Compare(x.Item2, y.Item2));

                userId = entry.Key;
                userName = entry.Value.Last().Item1;
                userState = entry.Value.Last().Item3;

                toReturn.Add((new UserSilhouette(userId, userName), userState));
            }
            return toReturn;
        }
        public async Task EndEventAsync(ulong id)
        {
            List<ulong> guests = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.CurrentEvent == id).
                Select(u => u.Id).
                ToListAsync());

            List<Task> tasks = new();
            foreach (ulong guest in guests)
            {
                tasks.Add(SetUserStateAsync(guest, id, EventBond.Left));
            }
            await Task.WhenAll(tasks);       

            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Event e = new() { Id = id, EndTime = DateTimeOffset.UtcNow, State = EventState.Ended };
            storeSentry.DiscussWrite(ctx => ctx.Events.Attach(e), currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(nameof(e.EndTime)).IsModified = true, currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(nameof(e.State)).IsModified = true, currentDiscussion);
            await storeSentry.EndDiscussionAsync(currentDiscussion);         
        }
    }
}

