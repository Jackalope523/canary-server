using Server.Boundaries;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetTopologySuite.Geometries;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using System.Security.Cryptography;
using Server.Entities;

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

        private TEntity ApplyEntityEdit<TEntity>(TEntity entity, Func<TEntity,object> edit)
        {
            edit(entity);
            return entity;
        }

        public bool CreateUser(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth) 
        { 
            User toCreate = new User
            {
                PhoneNumber = phoneNumber,
                Email = email,
                Name = name,
                DateOfBirth = dateOfBirth,
                JoinDate = DateTimeOffset.UtcNow,
                Reputation = 100,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            return EntityOperation(toCreate, u => _context.Users.Add((User)u)); 
        }
        public bool DeleteUser(Guid Id) { return EntityOperation(new User { Id = Id }, u => _context.Users.Remove((User)u)); }

        
        Func<Entity, EntityEntry> updateUser = u => _context.Users.Update((User)u);
        public bool UpdatePhoneNumber(Guid id, string newNumber) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.PhoneNumber = newNumber), updateUser); }
        public bool UpdateEmail(Guid id, string newEmail) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.Email = newEmail), updateUser); }
        public bool UpdateName(Guid id, string newName) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.Name = newName), updateUser); }
        public bool UpdatePhoneConfirmation(Guid id, bool isConfirmed) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.IsPhoneConfirmed = isConfirmed), updateUser); }
        public bool UpdateEmailConfirmation(Guid id, bool isConfirmed) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.IsEmailConfirmed = isConfirmed), updateUser); }
        public bool UpdateSecurityStamp(Guid id, string newSecurityStamp) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.SecurityStamp = newSecurityStamp), updateUser); }
        public bool UpdateLockoutDate(Guid id, DateTimeOffset? newLockoutDate) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.LockoutDate = newLockoutDate), updateUser); }
        public bool UpdateAccessTries(Guid id, int newAccessTries) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.AccessTries = newAccessTries), updateUser); }
        public bool UpdateReputation(Guid id, int newReputation) { return EntityOperation(ApplyEntityEdit(GetUser(id), u => u.Reputation = newReputation), updateUser); }

        Func<Entity, EntityEntry> addUserLink = l => _context.UserLinks.Add((UserLink)l);
        Func<Entity, EntityEntry> removeUserLink = l => _context.UserLinks.Remove((UserLink)l);
        public bool FollowUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, addUserLink); }      
        public bool UnfollowUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, removeUserLink); } 
        public bool BlockUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, addUserLink); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, removeUserLink); }          

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

        private User GetUser(Guid id)
        {
			User user;
			using (_context = new QueryContext())
			{
				user = _context.Users.Find(id);
			}
            if (user == null)
            { throw new InvalidUserException("User not found."); }
            return user;
		}
        private User GetUser(string phoneNumber)
        {
			User user;
			using (_context = new QueryContext())
			{
                try
                {
                    user = _context.Users.Where(u => u.PhoneNumber.Equals(phoneNumber)).Single();
                }
                catch (Exception ex)
                {
                    throw new InvalidUserException("User not found.", ex);
                }
			}
            return user;
		}
        private int GetUserFollowerCount(Guid id)
        {
			int numFollowers;
            using (_context = new QueryContext())
            {
				numFollowers = _context.UserLinks.Where(l => l.SelfId == id && l.Type.Equals(UserLink.UserLinkType.Following)).Count();
            }
            return numFollowers;
        }

        public ThinUser FindUser(Guid id)
        {
            User user = GetUser(id);
            int numFollowers = GetUserFollowerCount(id);
            return new ThinUser(user.Id, user.PhoneNumber, user.Email, user.Name, user.DateOfBirth,
                user.IsPhoneConfirmed, user.IsEmailConfirmed,
                user.SecurityStamp, user.LockoutDate, user.AccessTries,
                user.JoinDate, user.Reputation, numFollowers);
        }

        public ThinUser FindUser(string phoneNumber)
        {
            User user = GetUser(phoneNumber);
            int numFollowers = GetUserFollowerCount(user.Id);
            return new ThinUser(user.Id, user.PhoneNumber, user.Email, user.Name, user.DateOfBirth,
                user.IsPhoneConfirmed, user.IsEmailConfirmed,
                user.SecurityStamp, user.LockoutDate, user.AccessTries,
                user.JoinDate, user.Reputation, numFollowers);
        }


		private Event GetEvent(Guid id)
		{
			Event @event;
			using (_context = new QueryContext())
			{
				@event = _context.Events.Find(id);
			}
			if (@event == null)
			{ throw new InvalidEventException("Event not found."); }
			return @event;
		}
		public ThinEvent FindEvent(Guid id)
        {
            Event @event = GetEvent(id);
            ThinnerUser host = new ThinnerUser(@event.Host.Id, @event.Host.Name); 
            return new ThinEvent(@event.Id, host, @event.Name, @event.Description, @event.EventType,
                @event.StartTime, @event.Location.X, @event.Location.Y, @event.EndTime,
                @event.IsEventOpen, @event.GroupMinimum, @event.GroupMaximum);
        }

        public List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance)
        {
            List<ThinnerEvent> closestEvents;
            Point userLocation = new Point(longitude, latitude); 
            using (_context = new QueryContext())
            { 
                closestEvents = _context.Events.Where(e => e.Location.Distance(userLocation) <= distance && !e.EndTime.HasValue).
                                Select(e => new ThinnerEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.EventType, e.Location.Y, e.Location.X)).ToList();
            }
            return closestEvents;
        }

        public ThinEvent FindAttendingEvent(Guid id)
		{
			Event @event;
			ThinnerUser host;
			using (_context = new QueryContext())
			{
				@event = _context.EventLinks.Where(e => e.SelfId == id).Include(e => e.Event.Host).Single().Event;
				host = new ThinnerUser(@event.Host.Id, @event.Host.Name);
			}
            return new ThinEvent(@event.Id, host, @event.Name, @event.Description, @event.EventType,
                @event.StartTime, @event.Location.X, @event.Location.Y, @event.EndTime,
				@event.IsEventOpen, @event.GroupMinimum, @event.GroupMaximum);
		}

        public List<ThinEvent> FindUpcomingEvents(Guid id)
        {
            List<ThinEvent> upcomingEvents;
            using (_context = new QueryContext())
            { 
                upcomingEvents = _context.Events.Where(e => e.StartTime > DateTimeOffset.UtcNow).
								Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.EventType,
                                e.StartTime, e.Location.Y, e.Location.X, e.EndTime, e.IsEventOpen, e.GroupMinimum, e.GroupMaximum)).ToList();
            }
            return upcomingEvents;
        }

        public List<ThinEvent> FindPastEvents(Guid id)
		{
            List<ThinEvent> pastEvents;
            using (_context = new QueryContext())
            { 
                pastEvents = _context.Events.Where(e => e.EndTime.HasValue && e.EndTime < DateTimeOffset.UtcNow).
								Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.EventType,
                                e.StartTime, e.Location.Y, e.Location.X, e.EndTime, e.IsEventOpen, e.GroupMinimum, e.GroupMaximum)).ToList();
            }
            return pastEvents;
		}

        public ThinEvent CreateEvent(Guid hostId, string name, string description, string eventType,
            DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum)
        {
            Event toCreate = new Event {
                HostId = hostId,
                Name = name,
                EventType = eventType,
                StartTime = startTime,
                Location = new Point(longitude, latitude),

                IsEventOpen = true,
                GroupMinimum = groupMinimum,
                GroupMaximum = groupMaximum
            };
            EntityOperation(toCreate, e => _context.Events.Add((Event)e));
            AddUserToEvent(hostId, toCreate.Id);

            ThinUser host = FindUser(hostId);
            ThinnerUser thinHost = new(host.Id, host.Name);
			return new ThinEvent(toCreate.Id, thinHost, toCreate.Name, toCreate.Description, toCreate.EventType,
                toCreate.StartTime, toCreate.Location.X, toCreate.Location.Y, toCreate.EndTime,
				toCreate.IsEventOpen, toCreate.GroupMinimum, toCreate.GroupMaximum);
		}

		Func<Entity, EntityEntry> updateEvent = e => _context.Events.Update((Event)e);
		public bool UpdateDescription(Guid id, string description) { return EntityOperation(ApplyEntityEdit(GetEvent(id), e => e.Description = description), updateEvent); }
		public bool UpdateType(Guid id, string type) { return EntityOperation(ApplyEntityEdit(GetEvent(id), e => e.EventType = type), updateEvent); }
		public bool UpdateStatus(Guid id, bool isOpen) { return EntityOperation(ApplyEntityEdit(GetEvent(id), e => e.IsEventOpen = isOpen), updateEvent); }
        public bool EndEvent(Guid id) { return EntityOperation(ApplyEntityEdit(GetEvent(id), e => e.EndTime = DateTimeOffset.UtcNow), updateEvent); }

		Func<Entity, EntityEntry> addEventLink = l => _context.EventLinks.Add((EventLink)l);
		Func<Entity, EntityEntry> removeEventLink = l => _context.EventLinks.Remove((EventLink)l);

		public bool AddUserToEvent(Guid userId, Guid eventId) { return EntityOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attending }, addEventLink); }
        public bool RemoveUserFromEvent(Guid userId, Guid eventId) { return EntityOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attending }, removeEventLink); }    

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
