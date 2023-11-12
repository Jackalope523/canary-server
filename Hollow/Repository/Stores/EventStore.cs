using Core.Boundaries;
using NetTopologySuite.Geometries;

namespace Repository
{
    public class EventStore : QueryStore, IEventDatabase
    {
        public static IEventDatabase EventDatabaseAccess => new EventStore(new TestSentry());

        public EventStore(Sentry sentry) : base(sentry)
        {
        }

        public EventShard CreateEvent(Guid hostId, string name, string description, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum, Character character)
        {
            Event toCreate = new Event
            {
                HostId = hostId,
                Name = name,
                Description = description,
                StartTime = startTime,
                Location = new Point(longitude, latitude),
                GroupMinimum = groupMinimum,
                GroupMaximum = groupMaximum,
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,

            };

            storeSentry.ExecuteWrite(ctx => ctx.Events.Add(toCreate));


            return new EventShard
                (
                   toCreate.Id,
                   new UserSilhouette(toCreate.Host.Id, toCreate.Host.Name),
                   toCreate.Name,
                   toCreate.Description,
                   toCreate.StartTime,
                   toCreate.Location.Y,
                   toCreate.Location.X,
                   toCreate.EndTime,
                   toCreate.IsOpen,
                   toCreate.GroupMinimum,
                   toCreate.GroupMaximum,
                   new Character(
                   toCreate.Extroversion,
                   toCreate.Athleticisme,
                   toCreate.Chaos,
                   toCreate.Competitiveness,
                   toCreate.Industriousness,
                   toCreate.NightOwl,
                   toCreate.Openness));
        }
        public EventShard FindCurrentEventForUser(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Attend).Single(); }
        public List<EventShard> FindUpcomingEventsForUser(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Watching); }
        public List<EventShard> FindPastEventsForUser(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Left); }

        public EventShard FindEvent(Guid id)
        {
            EventShard @event = storeSentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Id == id).Select(e => new EventShard
               (
                   e.Id,
                   new UserSilhouette(e.HostId, null),
                   e.Name,
                   e.Description,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.IsOpen,
                   e.GroupMinimum,
                   e.GroupMaximum,
                   new Character(
                   e.Extroversion,
                   e.Athleticisme,
                   e.Chaos,
                   e.Competitiveness,
                   e.Industriousness,
                   e.NightOwl,
                   e.Openness)
               )).Single());

            UserSilhouette host = storeSentry.ExecuteRead(ctx => ctx.Users.Where(u => u.Id == @event.Host.Id).Select(u => new UserSilhouette(u.Id, u.Name)).Single()) ;

            return @event with {Host = host } ;
        }

        public List<EventThinSlice> FindEvents(double latitude, double longitude, double distance)
        {
            List<EventThinSlice> closestEvents;
            Point userLocation = new Point(longitude, latitude);

            closestEvents = storeSentry.ExecuteRead(ctx => ctx.Events.Where(e => e.Location.Distance(userLocation) <= distance && !e.EndTime.HasValue).
                                Join(ctx.Users, 
                                e => e.HostId, 
                                u => u.Id, 
                                (e,u) => new EventThinSlice(e.Id, new UserSilhouette(u.Id, u.Name), e.Location.Y, e.Location.X)).
                                ToList());

            return closestEvents;
        }
        
        public List<UserSilhouette> GetGuestList(Guid id)
        {
            List<UserSilhouette> guests;

            guests = storeSentry.ExecuteRead(ctx => ctx.EventLinks.Where(l => l.OtherId == id && l.Type == EventLink.EventLinkType.Attend).Select(l => new UserSilhouette(l.Self.Id, l.Self.Name)).ToList());

            return guests;
        }

        public bool AddUserToEvent(Guid userId, Guid eventId) { return addLinkOperation(new EventLink { SelfId = userId, OtherId = eventId, Type = EventLink.EventLinkType.Attend }); }
        public bool RemoveUserFromEvent(Guid userId, Guid eventId) { return removeLinkOperation(new EventLink { SelfId = userId, OtherId = eventId, Type = EventLink.EventLinkType.Attend }); }

        public bool UpdateEvent(Guid id, List<(string Property, object Value)> edits)
        {
            Event e = new Event { Id = id };
            storeSentry.DiscussWrite(ctx => ctx.Events.Attach(e));

            foreach ((string Property, object Value) edit in edits)
            {
                switch (edit.Property)
                {
                    case "Description":
                        e.Description = (string)edit.Value;
                        break;
                    case "IsOpen":
                        e.IsOpen = (bool)edit.Value;
                        break;
                    case "EndTime":
                        e.EndTime = (DateTimeOffset?)edit.Value;
                        break;
                    default:
                        throw new Exception("No propertyName match found");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(e).Property(edit.Property).IsModified = true);
            }
            storeSentry.ExecuteWrite();
            return true;
        }

        public bool EndEvent(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> GetGuestHistory(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
