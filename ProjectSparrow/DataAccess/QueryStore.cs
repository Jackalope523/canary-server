using Server.Boundaries;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetTopologySuite.Geometries;

namespace DataAccess
{ 
    public class QueryStore : IAccountDatabase, IEventDatabase
    {
        public static IAccountDatabase AccountDatabaseAccess => new QueryStore();
        public static IEventDatabase EventDatabaseAccess => new QueryStore();

        private static QueryContext _context = new QueryContext();

        // User Queries
        private static bool EntityOperation(Entity target, Func<Entity,EntityEntry> work)
        {
            int numWrites;
            using (_context = new QueryContext())
            {
                work(target);
                numWrites = _context.SaveChanges();
            }
            return numWrites > 0;       
        }      
        public bool CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth) 
        { 
            User toCreate = new User
            {
                PhoneNumber = phoneNumber,
                Passkey = passkey,
                Name = name,
                DateOfBirth = dateOfBirth,
                JoinDate = DateTime.Now,
                Reputation = 100
            };

            return EntityOperation(toCreate, u => _context.Users.Add((User)u)); 
        }
        public bool DeleteUser(Guid Id) { return EntityOperation(new User { Id = Id }, u => _context.Users.Remove((User)u)); }

        Func<Entity, EntityEntry> updateUser = u => _context.Users.Update((User)u);
        public bool UpdateName(Guid id, string newName) { return EntityOperation(new User { Id = id, Name = newName }, updateUser); }
        public bool UpdatePasskey(Guid id, string newPasskey) { return EntityOperation(new User { Id = id, Passkey = newPasskey }, updateUser); }
        public bool UpdatePhoneNumber(Guid id, string newNumber) { return EntityOperation(new User { Id = id, PhoneNumber = newNumber }, updateUser); }
        public bool UpdateReputation(Guid id, int newReputation) { return EntityOperation(new User { Id = id, Reputation = newReputation }, updateUser); }

        Func<Entity, EntityEntry> addLink = l => _context.UserLinks.Add((UserLink)l);
        Func<Entity, EntityEntry> removeLink = l => _context.UserLinks.Remove((UserLink)l);
        public bool FollowUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, addLink); }      
        public bool UnfollowUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, removeLink); } 
        public bool BlockUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, addLink); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, removeLink); }          

        private static List<ThinnerUser> GetCollectionOfUsers(Guid id, UserLink.UserLinkType type)
        {
            List<ThinnerUser> users;
            using (_context = new QueryContext())
            {
                users = _context.UserLinks.Where(l => l.SelfId == id && l.Type == type).Include(l => l.Other).Select(l => new ThinnerUser(l.Other.Id, l.Other.Name)).ToList();
            }
            return users;
        }
        public List<ThinnerUser> GetBlockedUsers(Guid id) { return GetCollectionOfUsers(id, UserLink.UserLinkType.Blocked); }
        public List<ThinnerUser> GetFollowedUsers(Guid id) { return GetCollectionOfUsers(id, UserLink.UserLinkType.Following); }

        public ThinUser FindUser(Guid id)
        {
            User user;
            int numFollowers;
            using (_context = new QueryContext())
            {
                user = _context.Users.Find(id);
                numFollowers = _context.UserLinks.Where(l => l.SelfId == user.Id && l.Type.Equals(UserLink.UserLinkType.Following)).Count();
            }
            return new ThinUser(user.Id, user.PhoneNumber, "", user.Name, user.DateOfBirth,
                true, true, "", null, 0, DateTimeOffset.Now, user.Reputation, numFollowers);
        }

        public ThinUser FindUser(string phoneNumber)
        {
            User user;
            int numFollowers;
            using (_context = new QueryContext())
            {
                user = _context.Users.Where(u => u.PhoneNumber.Equals(phoneNumber)).Single();
                numFollowers = _context.UserLinks.Where(l => l.SelfId == user.Id && l.Type.Equals(UserLink.UserLinkType.Following)).Count();
            }
            return new ThinUser(user.Id, user.PhoneNumber, "", user.Name, user.DateOfBirth,
				true, true, "", null, 0, DateTimeOffset.Now, user.Reputation, numFollowers);
        }

        public ThinEvent FindEvent(Guid id)
        {
            Event @event;
            ThinnerUser host;
            using (_context = new QueryContext())
            {
                @event = _context.Events.Where(e => e.Id == id).Include(e => e.Host).Single();
                host = new ThinnerUser(@event.Host.Id, @event.Host.Name); 
            }
            return new ThinEvent(@event.Id,  host, @event.Name, @event.EventType, @event.StartTime, @event.Location.X, @event.Location.Y);
        }

        public List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance)
        {
            List<ThinnerEvent> closestEvents;
            Point userLocation = new Point(longitude, latitude); 
            using (_context = new QueryContext())
            { 
                closestEvents = _context.Events.Where(e => e.Location.Distance(userLocation) <= distance).
                                Select(e => new ThinnerEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.EventType, e.Location.Y, e.Location.X)).ToList();
            }
            return closestEvents;
        }

        public bool CreateEvent(Guid hostId, string name, string eventType, DateTime startTime, double latitude, double longitude)
        {
            Event toCreate = new Event {
                HostId = hostId,
                Name = name,
                EventType = eventType,
                StartTime = startTime,
                Location = new Point(longitude, latitude), 
            };
            return EntityOperation(toCreate, e => _context.Events.Add((Event)e));
        }
        public bool EndEvent(Guid id) { return EntityOperation(new Event { Id = id }, e => _context.Events.Remove((Event)e)); }

        public bool AddUserToEvent(Guid userId, Guid eventId) { return EntityOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attending }, addLink); }

        public bool RemoveUserFromEvent(Guid userId, Guid eventId) { return EntityOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attending }, removeLink); }    

        public List<ThinnerUser> GetGuestList(Guid id)
        {
            List<ThinnerUser> guests;
            using (_context = new QueryContext())
            {
                guests = _context.EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Attending).Include(l => l.Self).Select(l => new ThinnerUser(l.Self.Id, l.Self.Name)).ToList();
            }
            return guests;
        }


    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
