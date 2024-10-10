using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;


namespace Repository
{
    public class EFCoreGatheringStore : QueryStore, IGatheringDatabase
    {
        public EFCoreGatheringStore(Harbor.Flag flag) : base(flag)
        {
        }

        private async Task PropagateClearance(ulong userId, ulong gatheringId, int degree, List<ulong> exclusionList)
        {
            if (degree == 0) return;

            ulong id = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.GuestClearances.
                Where(c => c.GatheringId == gatheringId && c.UserId == userId).
                Select(c => c.Id).
                SingleOrDefaultAsync());

            if (id != 0)
            {
               await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.GuestClearances.
               AddAsync(new GuestClearance
               {
                   UserId = userId,
                   GatheringId = gatheringId,
                   Degree = degree,
               }
               ));
            }
            else return;

            Task<List<ulong>> appreciating = storeSentry.ExecuteReadAsync(ctx =>
                ctx.UserLinks.
                Where(l => !exclusionList.Contains(l.OtherId) && l.SelfId == userId && l.Type == UserRelationship.UserLinkType.Appreciate).
                Select(l => l.OtherId).
                ToListAsync());

            Task<List<ulong>> appreciatingMe = storeSentry.ExecuteReadAsync(ctx =>
                ctx.UserLinks.
                Where(l => !exclusionList.Contains(l.SelfId) && l.OtherId == userId && l.Type == UserRelationship.UserLinkType.Appreciate).
                Select(l => l.SelfId).
                ToListAsync());

            List<ulong> companions = (await appreciating).Intersect(await appreciatingMe).ToList();

            List<Task> tasks = new();
            foreach (ulong companion in companions)
            {
                tasks.Add(PropagateClearance(companion, gatheringId, degree - 1, companions.Union(exclusionList).ToList()));
            }
            await Task.WhenAll(tasks);
        }

        private async Task UpdateClearance(ulong gatheringId, int previousDegreeOfPrivacy, int newDegreeOfPrivacy)
        {
            if (previousDegreeOfPrivacy < newDegreeOfPrivacy)
            {
                List<ulong> edgeUsers = await storeSentry.ExecuteReadAsync(ctx =>
                    ctx.GuestClearances.
                    Where(c => c.GatheringId == gatheringId && c.Degree == previousDegreeOfPrivacy).
                    Select(c => c.UserId).
                    ToListAsync());


                List<Task> tasks = new();
                foreach (ulong user in edgeUsers)
                {
                    tasks.Add(PropagateClearance(user, gatheringId, newDegreeOfPrivacy - previousDegreeOfPrivacy, new(edgeUsers)));
                }
                await Task.WhenAll(tasks);
            }
            else if (previousDegreeOfPrivacy > newDegreeOfPrivacy)
            {
                await storeSentry.ExecuteWriteAsync(ctx =>
                    ctx.GuestClearances.
                    Where(c => c.GatheringId == gatheringId && c.Degree > newDegreeOfPrivacy).
                    ExecuteDeleteAsync());
            }
            else
            {
                return;
            }
        }

        public async Task<CoreGathering> CreateGatheringAsync(ulong hostId, string title, string description, DateTimeOffset startTime, double latitude, double longitude, string friendlyLocation, int groupMinimum, int groupMaximum, CharacterShard character, double Radius, bool isDynamic, int degreeOfPrivacy)
        {
            Gathering toCreate = new()
            {
                Name = title,
                Description = description,
                StartTime = startTime,
                HostId = hostId,
                Location = new CoordinateFactory().Create(longitude, latitude),
                FriendlyLocation = friendlyLocation,
                GroupMinimum = groupMinimum,
                GroupMaximum = groupMaximum,
                Radius = Radius,
                IsDynamic = isDynamic,
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,
                DegreeOfPrivacy = degreeOfPrivacy,
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Gatherings.Add(toCreate));

            GatheringLink hostLink = new() 
            { 
                UserId = hostId,
                GatheringId = toCreate.Id,
                Type = GatheringBond.Guest,      
                Time = DateTimeOffset.UtcNow
            };
            
            await SetUserStateAsync(hostId, toCreate.Id, GatheringBond.Guest, DateTimeOffset.UtcNow);

            UserShard host = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == hostId).Select(u => new UserShard(u.Id, u.Name)).SingleAsync());

            if (degreeOfPrivacy < 3)
            {
                await PropagateClearance(hostId, toCreate.Id, degreeOfPrivacy, new());
            }

            return new CoreGathering
                (
                   toCreate.Id,
                   host,
                   toCreate.Name,
                   toCreate.Description,
                   toCreate.StartTime,
                   toCreate.Location.Y,
                   toCreate.Location.X,
                   toCreate.FriendlyLocation,
                   toCreate.EndTime,
                   toCreate.State,
                   toCreate.GroupMinimum,
                   toCreate.GroupMaximum,
                   new CharacterShard(
                   toCreate.Age,
                   toCreate.Extroversion,
                   toCreate.Athleticisme,
                   toCreate.Chaos,
                   toCreate.Competitiveness,
                   toCreate.Industriousness,
                   toCreate.NightOwl,
                   toCreate.Openness),
                   toCreate.Radius,
                   toCreate.IsDynamic,
                   toCreate.IsPendingDeletion,
                   toCreate.NumberOfGuests,
                   toCreate.DegreeOfPrivacy
                   );
        }
        public async Task DeleteGatheringAsync(ulong gatheringId)
        {
            await storeSentry.ExecuteWriteAsync(ctx => 
                ctx.Gatherings.
                Where(e => e.Id == gatheringId).
                ExecuteUpdate(setter => setter.SetProperty(e => e.IsPendingDeletion, true)));
        }
        public async Task<CoreGathering> FindCurrentGatheringForUserAsync(ulong id) 
        {
            ulong? currentGathering = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.Users.
            Where(u => u.Id == id).
            Select(u => u.CurrentGathering).
            SingleAsync());

            CoreGathering? gathering = null;
            if (currentGathering != null)
            {
                gathering = await storeSentry.ExecuteReadAsync(ctx =>
                    ctx.Gatherings.
                    Where(e => e.Id == currentGathering).
                    Select(e => new CoreGathering
                    (
                        e.Id,
                        new UserShard(e.HostId, null),
                        e.Name,
                        e.Description,
                        e.StartTime,
                        e.Location.Y,
                        e.Location.X,
                        e.FriendlyLocation,
                        e.EndTime,
                        e.State,
                        e.GroupMinimum,
                        e.GroupMaximum,
                        new CharacterShard(
                        e.Extroversion,
                        e.Athleticisme,
                        e.Chaos,
                        e.Competitiveness,
                        e.Industriousness,
                        e.NightOwl,
                        e.Openness,
                        e.Age),
                        e.Radius,
                        e.IsDynamic,
                        e.IsPendingDeletion,
                        e.NumberOfGuests,
                        e.DegreeOfPrivacy
                        )).SingleAsync());

                UserShard host = await storeSentry.ExecuteReadAsync(ctx =>
                ctx.Users.
                Where(u => u.Id == gathering.Host.Id).
                Select(u => new UserShard(u.Id, u.Name)).
                SingleAsync());


                gathering =  gathering with { Host = host };
            }
            return gathering;          
        }
        public async Task<List<CoreGathering>> FindUpcomingGatheringsForUserAsync(ulong id) 
        {
             List<CoreGathering> upcomingGatherings = await storeSentry.ExecuteReadAsync(ctx =>
             ctx.GatheringLinks.
             Where(l => l.UserId == id && l.Type == GatheringBond.Guest).
             Join(
                ctx.Gatherings.Where(e => (e.State == GatheringState.Upcoming || e.State == GatheringState.OngoingOpen) && !e.IsPendingDeletion),
                l => l.GatheringId,
                e => e.Id,
                (l, e) => new
                {
                    e.Id,
                    e.Name,
                    e.HostId,
                    e.Description,
                    e.StartTime,
                    e.Location,
                    e.FriendlyLocation,
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
                    e.Age,
                    e.Openness,
                    e.Radius,
                    e.IsDynamic,
                    e.IsPendingDeletion,
                    e.NumberOfGuests,
                    e.DegreeOfPrivacy
                }).
             Join(
                 ctx.Users,
                 e => e.HostId,
                 u => u.Id,
                 (e, u) => new CoreGathering
                 (
                    e.Id,
                    new UserShard(u.Id, u.Name),
                    e.Name,
                    e.Description,
                    e.StartTime,
                    e.Location.Y,
                    e.Location.X,
                    e.FriendlyLocation,
                    e.EndTime,
                    e.State,
                    e.GroupMinimum,
                    e.GroupMaximum,
                    new CharacterShard(
                    e.Age,
                    e.Extroversion,
                    e.Athleticisme,
                    e.Chaos,
                    e.Competitiveness,
                    e.Industriousness,
                    e.NightOwl,
                    e.Openness),
                    e.Radius,
                    e.IsDynamic,
                    e.IsPendingDeletion,
                    e.NumberOfGuests,
                    e.DegreeOfPrivacy
                 )).ToListAsync());

            List<ulong> toExclude = await storeSentry.ExecuteReadAsync(ctx =>
               ctx.GatheringLinks.
               Where(l => l.UserId == id && l.Type == GatheringBond.Arrived).
               Select(l => l.GatheringId). 
               ToListAsync());

            for (int i = 0; i < upcomingGatherings.Count; i++)
            {
                if (toExclude.Contains(upcomingGatherings[i].Id))
                {
                    upcomingGatherings.RemoveAt(i);
                    i--;
                }
            }

            return upcomingGatherings;
        }
        public async Task<List<CoreGathering>> FindSurveyingGatheringsForUserAsync(ulong id) 
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
             ctx.GatheringLinks.
             Where(l => l.UserId == id && l.Type == GatheringBond.Watching).
             Join(
                ctx.Gatherings.Where(e => !e.IsPendingDeletion),
                l => l.GatheringId,
                e => e.Id,
                (l, e) => new
                {
                    e.Id,
                    e.Name,
                    e.HostId,
                    e.Description,
                    e.StartTime,
                    e.Location,
                    e.FriendlyLocation,
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
                    e.Age,
                    e.Openness,
                    e.Radius,
                    e.IsDynamic,
                    e.IsPendingDeletion,
                    e.NumberOfGuests,
                    e.DegreeOfPrivacy
                }).
             Join(
                 ctx.Users,
                 e => e.HostId,
                 u => u.Id,
                 (e, u) => new CoreGathering
                 (
                    e.Id,
                    new UserShard(u.Id, u.Name),
                    e.Name,
                    e.Description,
                    e.StartTime,
                    e.Location.Y,
                    e.Location.X,
                    e.FriendlyLocation,
                    e.EndTime,
                    e.State,
                    e.GroupMinimum,
                    e.GroupMaximum,
                    new CharacterShard(
                    e.Age,
                    e.Extroversion,
                    e.Athleticisme,
                    e.Chaos,
                    e.Competitiveness,
                    e.Industriousness,
                    e.NightOwl,
                    e.Openness),
                    e.Radius,
                    e.IsDynamic,
                    e.IsPendingDeletion,
                    e.NumberOfGuests,
                    e.DegreeOfPrivacy
                 )).ToListAsync());
        }
        public async Task<List<CoreGathering>> FindPastGatheringsForUserAsync(ulong id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.GatheringLinks.
            Where(l => l.UserId == id && l.Type == GatheringBond.Left).
            Join(
               ctx.Gatherings.Where(e => !e.IsPendingDeletion),
               l => l.GatheringId,
               e => e.Id,
               (l, e) => new
               {
                  e.Id,               
                  e.Name,
                  e.HostId,
                  e.Description,
                  e.StartTime,
                  e.Location,
                  e.FriendlyLocation,
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
                  e.Age,
                  e.Openness,
                  e.Radius,
                  e.IsDynamic,
                  e.IsPendingDeletion,
                  e.NumberOfGuests,
                  e.DegreeOfPrivacy
               }).
            Join(
                ctx.Users,
                e => e.HostId,
                u => u.Id,
                (e, u) => new CoreGathering
                (
                   e.Id,
                   new UserShard(u.Id, u.Name),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.FriendlyLocation,
                   e.EndTime,
                   e.State,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new CharacterShard(
                   e.Age,
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness),
                   e.Radius,
                   e.IsDynamic,
                   e.IsPendingDeletion,
                   e.NumberOfGuests,
                   e.DegreeOfPrivacy
                )).ToListAsync());
        }
        public async Task<CoreGathering> FindGatheringAsync(ulong id)
        {
            CoreGathering gathering = await storeSentry.ExecuteReadAsync(ctx => 
            ctx.Gatherings.
            Where(e => e.Id == id).
            Select(e => new CoreGathering
               (
                   e.Id,
                   new UserShard(e.HostId, null),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.FriendlyLocation,
                   e.EndTime,
                   e.State,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new CharacterShard(
                   e.Age,
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness),
                   e.Radius,
                   e.IsDynamic,
                   e.IsPendingDeletion,
                   e.NumberOfGuests,
                   e.DegreeOfPrivacy
               )).
             SingleAsync());

            UserShard host = await storeSentry.ExecuteReadAsync(ctx => ctx.Users.Where(u => u.Id == gathering.Host.Id).Select(u => new UserShard(u.Id, u.Name)).SingleAsync()) ;

            return gathering with {Host = host } ;
        }
        public async Task<List<CoreGathering>> FindGatheringsAsync(double latitude, double longitude, double distance)
        {
            Point currentLocation = new CoordinateFactory().Create(longitude, latitude);
            DateTimeOffset today = DateTimeOffset.UtcNow;
            DateTimeOffset inTwoWeeks = today.AddDays(14);

            return await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Gatherings.
                Where(e => e.Location.Distance(currentLocation) <= distance && (e.State == GatheringState.OngoingOpen || e.State == GatheringState.Upcoming) && !e.IsPendingDeletion).
                Join(
                    ctx.Users, 
                    e => e.HostId, 
                    u => u.Id, 
                    (e,u) => new CoreGathering
                    (
                        e.Id,
                        new UserShard(u.Id, u.Name),
                        e.Name,
                        e.Description,
                        e.StartTime,
                        e.Location.Y,
                        e.Location.X,
                        e.FriendlyLocation,
                        e.EndTime,
                        e.State,
                        e.GroupMinimum,
                        e.GroupMaximum,
                        new CharacterShard(
                        e.Age,
                        e.Extroversion,
                        e.Athleticisme,
                        e.Chaos,
                        e.Competitiveness,
                        e.Industriousness,
                        e.NightOwl,
                        e.Openness),
                        e.Radius,
                        e.IsDynamic,
                        e.IsPendingDeletion,
                        e.NumberOfGuests,
                        e.DegreeOfPrivacy
                   )).ToListAsync());
        }     
        public async Task<List<UserShard>> GetGuestListAsync(ulong id)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
            ctx.GatheringLinks.
            Where(l => l.GatheringId == id && l.Type == GatheringBond.Guest).
            Join(
                ctx.Users,
                l => l.UserId,
                u => u.Id,
                (_,u) => new UserShard(u.Id, u.Name)
                ).
            ToListAsync());
        }    
        public async Task DeleteUserStateAsync(ulong userId, ulong gatheringId) 
        { 
            GatheringLink link = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.GatheringLinks.
                Where(l => l.UserId == userId && l.GatheringId == gatheringId).
                SingleAsync());

            Discussion currentDiscussion = storeSentry.BeginDiscussion();
            storeSentry.DiscussWrite(ctx => ctx.GatheringLinks.Remove(link), currentDiscussion);

            switch (link.Type)
            {
                case GatheringBond.Watching:
                    break;
                case GatheringBond.Guest:
                    int num = await storeSentry.ExecuteReadAsync(ctx =>
                        ctx.Gatherings.
                        Where(e => e.Id == gatheringId).
                        Select(e => e.NumberOfGuests).
                        SingleAsync());

                    Gathering e = new() { Id = gatheringId, NumberOfGuests = num - 1 };
                    storeSentry.DiscussWrite(ctx => ctx.Gatherings.Attach(e), currentDiscussion);
                    storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(nameof(e.NumberOfGuests)).IsModified = true, currentDiscussion);
                    break;
                case GatheringBond.Arrived:
                    User arrivedUser = new() { Id = userId, CurrentGathering = null };

                    storeSentry.DiscussWrite(ctx => ctx.Users.Attach(arrivedUser), currentDiscussion);
                    storeSentry.DiscussWrite(ctx => ctx.Entry(arrivedUser).Property(nameof(arrivedUser.CurrentGathering)).IsModified = true, currentDiscussion);
                    break;
                case GatheringBond.Left:
                    break;
                case GatheringBond.Kicked:
                    break;
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }
        public async Task UpdateGatheringAsync(ulong id, List<(string Property, object Value)> edits)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Gathering e = new() { Id = id };
            storeSentry.DiscussWrite(ctx => ctx.Gatherings.Attach(e), currentDiscussion);

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case nameof(CoreGathering.Title):
                        e.Name = (string)Value;
                        break;
                    case nameof(CoreGathering.Description):
                        e.Description = (string)Value;
                        break;
                    case nameof(CoreGathering.State):
                        e.State = (GatheringState)Value;
                        break;
                    case nameof(CoreGathering.StartTime):
                        e.StartTime = (DateTimeOffset)Value;
                        break;
                    case "Location":
                        var Location = ((double, double))Value;
                        e.Location = new CoordinateFactory().Create(Location.Item2, Location.Item1);
                        break;
                    case nameof(CoreGathering.FriendlyLocation):
                        e.FriendlyLocation = (string)Value;
                        break;
                    case nameof(CoreGathering.Radius):
                        e.Radius = (double)Value;
                        break;
                    case nameof(CoreGathering.IsDynamic):
                        e.IsDynamic = (bool)Value;
                        break;
                    case nameof(CoreGathering.GroupMinimum):
                        e.GroupMinimum = (int)Value;
                        break;
                    case nameof(CoreGathering.GroupMaximum):
                        e.GroupMaximum = (int)Value;
                        break;
                    case nameof(CoreGathering.DegreeOfPrivacy):
                        int prev = await storeSentry.ExecuteReadAsync(ctx => 
                                        ctx.Gatherings.
                                        Where(g => g.Id == id).
                                        Select(g => g.DegreeOfPrivacy).
                                        SingleAsync());

                        e.DegreeOfPrivacy = (int)Value;
                        await UpdateClearance(id, prev, e.DegreeOfPrivacy);
                        break;
                    default:
                        throw new InvalidInputException($"Property named \"{Property}\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(Property).IsModified = true, currentDiscussion);
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }   
        public async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, UserShard User)>> GetGuestHistoryAsync(ulong id)
        {
            var times = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.GatheringLinks.
            Where(l => l.GatheringId == id && (l.Type == GatheringBond.Arrived || l.Type == GatheringBond.Left)).
            Join(
                ctx.Users,
                l => l.UserId,
                u => u.Id,
                (l,u) => new { u.Id, u.Name, l.Time, l.Type }
                ).
            ToListAsync());

            Dictionary<ulong,List<(string, DateTimeOffset, GatheringBond)>> history = new();
            foreach (var item in times)
            {
                if (!history.ContainsKey(item.Id)) history.Add(item.Id, new());
                history[item.Id].Add((item.Name, item.Time, item.Type));               
            }

            List<(DateTimeOffset Joined, DateTimeOffset? Left, UserShard User)> toReturn = new();
            ulong userId;
            string userName;
            DateTimeOffset arrivalTime;
            DateTimeOffset? departureTime;
            foreach ( var entry in history)
            {
                entry.Value.Sort((x,y) => DateTimeOffset.Compare(x.Item2, y.Item2));

                (string firstName, DateTimeOffset firstTime, _) = entry.Value.First();
                (_, DateTimeOffset lastTime, GatheringBond lastState) = entry.Value.Last();

                userId = entry.Key;
                userName = firstName;
                arrivalTime = firstTime;
                if (lastState == GatheringBond.Left) departureTime = lastTime;
                else departureTime = null;
                
                toReturn.Add((arrivalTime, departureTime, new UserShard(userId, userName)));
            }

            return toReturn;
        }      
        public async Task<List<CoreGathering>> FindGatheringsByUserAsync(ulong userId)
        {
           return await storeSentry.ExecuteReadAsync(ctx =>
           ctx.Gatherings.
           Where(e => e.HostId == userId && !e.IsPendingDeletion).
           Join(
               ctx.Users,
               e => e.HostId,
               u => u.Id,
               (e, u) => new CoreGathering(
                    e.Id, 
                    new UserShard(u.Id, u.Name), 
                    e.Name, 
                    e.Description, 
                    e.StartTime, 
                    e.Location.Y, 
                    e.Location.X, 
                    e.FriendlyLocation,
                    e.EndTime, 
                    e.State, 
                    e.GroupMinimum, 
                    e.GroupMaximum, 
                    new CharacterShard(
                        e.Extroversion,
                        e.Athleticisme, 
                        e.Chaos, 
                        e.Competitiveness,
                        e.Industriousness, 
                        e.NightOwl,
                        e.Openness,
                        e.Age
                        ), 
                    e.Radius, 
                    e.IsDynamic,
                    e.IsPendingDeletion,
                    e.NumberOfGuests,
                    e.DegreeOfPrivacy
                    )).ToListAsync());
        }      
        public async Task<GatheringBond?> GetUserStateAsync(ulong userId, ulong gatheringId)
        {
            var states = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.GatheringLinks.
            Where(l => l.UserId == userId && l.GatheringId == gatheringId).
            Select(l => new { l.Type, l.Time }).
            ToListAsync());

            states.Sort((x, y) => DateTimeOffset.Compare(x.Time, y.Time));

            return states.Last().Type;
        }
        public async Task SetUserStateAsync(ulong userId, ulong gatheringId, GatheringBond userState, DateTimeOffset time)
        {
            GatheringLink toAdd = new() 
            { 
                UserId = userId, 
                GatheringId = gatheringId, 
                Type = userState, 
                Time = time
            };

            ulong id = await storeSentry.ExecuteReadAsync(ctx => 
                       ctx.GatheringLinks.
                       Where(l => l.UserId == userId && l.GatheringId == gatheringId && l.Type == userState).
                       Select(l => l.Id).
                       SingleOrDefaultAsync());

            if (id != 0)
            {
                toAdd.Id = id;
            }

            Discussion currentDiscussion = storeSentry.BeginDiscussion();
            storeSentry.DiscussWrite(ctx => ctx.GatheringLinks.Update(toAdd), currentDiscussion);

            switch (userState)
            {
                case GatheringBond.Watching:
                    break;
                case GatheringBond.Guest:
                    int num = await storeSentry.ExecuteReadAsync(ctx => 
                        ctx.Gatherings.
                        Where(e => e.Id == gatheringId).
                        Select(e => e.NumberOfGuests).
                        SingleAsync());

                    Gathering e = new() { Id = gatheringId, NumberOfGuests = num + 1};
                    storeSentry.DiscussWrite(ctx => ctx.Gatherings.Attach(e), currentDiscussion);
                    storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(nameof(e.NumberOfGuests)).IsModified = true, currentDiscussion);
                    break;
                case GatheringBond.Arrived:
                    User arrivedUser = new() { Id = userId, CurrentGathering = gatheringId };

                    storeSentry.DiscussWrite(ctx => ctx.Users.Attach(arrivedUser), currentDiscussion);
                    storeSentry.DiscussWrite(ctx => ctx.Entry(arrivedUser).Property(nameof(arrivedUser.CurrentGathering)).IsModified = true, currentDiscussion);
                    break;
                case GatheringBond.Left:
                    User leftUser = new() { Id = userId, CurrentGathering = null };

                    storeSentry.DiscussWrite(ctx => ctx.Users.Attach(leftUser), currentDiscussion);
                    storeSentry.DiscussWrite(ctx => ctx.Entry(leftUser).Property(nameof(leftUser.CurrentGathering)).IsModified = true, currentDiscussion);
                    break;
                case GatheringBond.Kicked:
                    User kickedUser = new() { Id = userId, CurrentGathering = null };

                    storeSentry.DiscussWrite(ctx => ctx.Users.Attach(kickedUser), currentDiscussion);
                    storeSentry.DiscussWrite(ctx => ctx.Entry(kickedUser).Property(nameof(kickedUser.CurrentGathering)).IsModified = true, currentDiscussion);
                    break;
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }
        public async Task<List<(UserShard User, GatheringBond State)>> GetAllUsersAsync(ulong gatheringId)
        {
            var users = await storeSentry.ExecuteReadAsync(ctx =>
            ctx.GatheringLinks.
            Where(l => l.GatheringId == gatheringId).
            Join(
                ctx.Users,
                l => l.UserId,
                u => u.Id,
                (l, u) => new { u.Id, u.Name, l.Time, l.Type }
                ).
            ToListAsync());

            Dictionary<ulong, List<(string, DateTimeOffset, GatheringBond)>> history = new();
            foreach (var item in users)
            {
                if (!history.ContainsKey(item.Id)) history.Add(item.Id, new());
                history[item.Id].Add((item.Name, item.Time, item.Type));
            }

            List<(UserShard User, GatheringBond State)> toReturn = new();
            ulong userId;
            string userName;
            GatheringBond userState;
            foreach (var entry in history)
            {
                entry.Value.Sort((x, y) => DateTimeOffset.Compare(x.Item2, y.Item2));

                userId = entry.Key;
                userName = entry.Value.Last().Item1;
                userState = entry.Value.Last().Item3;

                toReturn.Add((new UserShard(userId, userName), userState));
            }
            return toReturn;
        }
        public async Task TerminateGatheringAsync(ulong id, DateTimeOffset time)
        {
            List<ulong> guests = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.Users.
                Where(u => u.CurrentGathering == id).
                Select(u => u.Id).
                ToListAsync());

            List<Task> tasks = new();
            foreach (ulong guest in guests)
            {
                tasks.Add(SetUserStateAsync(guest, id, GatheringBond.Left, time));
            }
            await Task.WhenAll(tasks);       

            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Gathering e = new() { Id = id, EndTime = DateTimeOffset.UtcNow, State = GatheringState.Ended };
            storeSentry.DiscussWrite(ctx => ctx.Gatherings.Attach(e), currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(nameof(e.EndTime)).IsModified = true, currentDiscussion);
            storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(nameof(e.State)).IsModified = true, currentDiscussion);

            await storeSentry.EndDiscussionAsync(currentDiscussion);         
        }

        public async Task<bool> UserIsAuthorizedGuest(ulong userId, ulong gatheringId)
        {
            ulong id = await storeSentry.ExecuteReadAsync(ctx => 
                ctx.GuestClearances.
                Where(c => c.GatheringId == gatheringId && c.UserId == userId).
                Select(c => c.Id).
                SingleOrDefaultAsync());
            
            return id != 0;
        }

        public async Task<List<ulong>> GetAuthorizedGuests(ulong gatheringId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
                ctx.GuestClearances.
                Where(c => c.GatheringId == gatheringId).
                Select(c => c.UserId).
                ToListAsync());
        }

        public async Task AddGuestAuthorization(ulong gatheringId, ulong userId)
        {
            GuestClearance toAdd = new()
            {
                GatheringId = gatheringId,
                UserId = userId,
                Degree = 0,
            };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.GuestClearances.Add(toAdd)); 
        }
    }
}

