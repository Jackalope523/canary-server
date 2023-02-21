using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetTopologySuite.Geometries;
using PhoneNumbers;
using Server.Boundaries;
using Shared;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;

namespace DataAccess
{
    public class QueryStore : IAccountDatabase, IEventDatabase
    {
        public static IAccountDatabase AccountDatabaseAccess => new QueryStore();
        public static IEventDatabase EventDatabaseAccess => new QueryStore();

        private static QueryContext _context = new QueryContext();

        // User Queries
        private static bool DatabaseOperation(Entity target, Func<Entity,EntityEntry> work)
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
                NormalisedEmail = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                AccountStatus = UserAccountStatus.active,
            };

            return DatabaseOperation(toCreate, u => _context.Users.Add((User)u)); 
        }
        public bool DeleteUser(Guid Id) { return DatabaseOperation(new User { Id = Id }, u => _context.Users.Remove((User)u)); }


        Func<Entity, EntityEntry> addUserLink = l => _context.UserLinks.Add((UserLink)l);
        Func<Entity, EntityEntry> removeUserLink = l => _context.UserLinks.Remove((UserLink)l);
        public bool FollowUser(Guid selfId, Guid targetId) { return DatabaseOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }, addUserLink); }      
        public bool UnfollowUser(Guid selfId, Guid targetId) { return DatabaseOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }, removeUserLink); } 
        public bool BlockUser(Guid selfId, Guid targetId) { return DatabaseOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }, addUserLink); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return DatabaseOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }, removeUserLink); }

        Func<Entity, EntityEntry> addEventLink = l => _context.EventLinks.Add((EventLink)l);
        Func<Entity, EntityEntry> removeEventLink = l => _context.EventLinks.Remove((EventLink)l);

        public bool AddUserToEvent(Guid userId, Guid eventId) { return DatabaseOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attend }, addEventLink); }
        public bool RemoveUserFromEvent(Guid userId, Guid eventId) { return DatabaseOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attend }, removeEventLink); }
        private static List<ThinnerUser> GetCollectionOfUsers(Guid id, UserLink.UserLinkType type)
        {
            List<ThinnerUser> users;
            using (_context = new QueryContext())
            {
                users = _context.UserLinks.Where(l => l.SelfId == id && l.Type == type).Include(l => l.Other).Select(l => new ThinnerUser(l.Other.Id, l.Other.Name)).ToList();
            }
            return users;
        }
        public List<ThinnerUser> GetFriends(Guid id) { return GetCollectionOfUsers(id, UserLink.UserLinkType.Follow); }
        public List<ThinnerUser> GetBlockedUsers(Guid id) { return GetCollectionOfUsers(id, UserLink.UserLinkType.Block); }
        public List<ThinnerUser> GetFollowedUsers(Guid id) { return GetCollectionOfUsers(id, UserLink.UserLinkType.Follow); }

		public (List<UserReport>, List<EventReport>) GetReports(Guid id) { throw new NotImplementedException(); }
		public (List<UserReport>, List<EventReport>) GetReportsByUser(Guid id) { throw new NotImplementedException(); }
		public bool ReportUser(Guid selfId, Guid targetId, UserReportType reportType, string reportDetails) { throw new NotImplementedException(); }

        private ThinUser FindUserBy(Func<User,bool> predicate)
        {
            ThinUser user;
            int numFollowers;
            using (_context = new QueryContext())
            {
                user = _context.Users.Where(predicate).Select(u => new ThinUser
                (
                    u.Id,
                    u.PhoneNumber,
                    u.Email,
                    u.Name,
                    u.DateOfBirth,
                    u.IsPhoneConfirmed,
                    u.IsEmailConfirmed,
                    u.SecurityStamp,
                    u.LockoutDate,
                    u.AccessTries,
                    u.AccountStatus,
                    u.JoinDate,
                    u.Reputation,
                    -1
                )).Single();

                numFollowers = _context.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).Count();

            }
            return user with { NumberOfFollowers = numFollowers};
        }

        public ThinUser FindUserById(Guid id) { return FindUserBy(u => u.Id == id); }
        public ThinUser FindUserByPhoneNumber(string phoneNumber) { return FindUserBy(u => u.PhoneNumber == phoneNumber); }
        public ThinUser FindUserByEmail(string email) { return FindUserBy(u => u.NormalisedEmail == email); }



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
            return @event.ToThinEvent();
        }

        public List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance)
        {
            List<ThinnerEvent> closestEvents;
            Point userLocation = new Point(longitude, latitude); 
            using (_context = new QueryContext())
            { 
                closestEvents = _context.Events.Where(e => e.Location.Distance(userLocation) <= distance && !e.EndTime.HasValue).
                                Select(e => new ThinnerEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Type, e.Location.Y, e.Location.X)).ToList();
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
            return new ThinEvent(@event.Id, host, @event.Name, @event.Description, @event.Type,
                @event.StartTime, @event.Location.X, @event.Location.Y, @event.EndTime,
				@event.IsEventOpen, @event.GroupMinimum, @event.GroupMaximum);
		}

        public List<ThinEvent> FindUpcomingEvents(Guid id)
        {
            List<ThinEvent> upcomingEvents;
            using (_context = new QueryContext())
            { 
                upcomingEvents = _context.Events.Where(e => e.StartTime > DateTimeOffset.UtcNow).
								Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.Type,
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
								Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.Type,
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
                Type = eventType,
                StartTime = startTime,
                Location = new Point(longitude, latitude),

                IsEventOpen = true,
                GroupMinimum = groupMinimum,
                GroupMaximum = groupMaximum
            };
            DatabaseOperation(toCreate, e => _context.Events.Add((Event)e));
            AddUserToEvent(hostId, toCreate.Id);

			return toCreate.ToThinEvent();
		}

        private static bool updateUserProperty(Guid id, string propertyName, Object newProperty)
        {
            User u = new User { Id = id };

            switch (propertyName)
            {
                case nameof(u.PhoneNumber):
                    u.PhoneNumber = (string)newProperty;
                    break;
                case nameof(u.Email):
                    u.Email = (string)newProperty;
                    break;
                case nameof(u.NormalisedEmail):
                    u.NormalisedEmail = (string)newProperty;
                    break;
                case nameof(u.Name):
                    u.Name = (string)newProperty;
                    break;
                case nameof(u.IsPhoneConfirmed):
                    u.IsPhoneConfirmed = (bool)newProperty;
                    break;
                case nameof(u.IsEmailConfirmed):
                    u.IsEmailConfirmed = (bool)newProperty;
                    break;
                case nameof(u.SecurityStamp):
                    u.SecurityStamp = (string)newProperty;
                    break;
                case nameof(u.LockoutDate):
                    u.LockoutDate = (DateTimeOffset?)newProperty;
                    break;
                case nameof(u.AccessTries):
                    u.AccessTries = (int)newProperty;
                    break;
                case nameof(u.AccountStatus):
                    u.AccountStatus = (UserAccountStatus)newProperty;
                    break;
                case nameof(u.Reputation):
                    u.Reputation = (int)newProperty;
                    break;
                default:
                    throw new Exception("No propertyName match found");
            }
            using (_context = new QueryContext())
            {
                _context.Users.Attach(u);
                _context.Entry(u).Property<string>(propertyName).IsModified = true;
                _context.SaveChanges();
            }
            return true;
        }
        public bool UpdatePhoneNumber(Guid id, string newNumber) { return updateUserProperty(id, nameof(User.PhoneNumber), newNumber); }
        public bool UpdateEmail(Guid id, string newEmail) { return updateUserProperty(id, nameof(User.Email), newEmail); }
        public bool UpdateNormalisedEmail(Guid id, string newNormalisedEmail) { return updateUserProperty(id, nameof(User.NormalisedEmail), newNormalisedEmail); }
        public bool UpdateName(Guid id, string newName) { return updateUserProperty(id, nameof(User.Name), newName); }
        public bool UpdatePhoneConfirmation(Guid id, bool isConfirmed) { return updateUserProperty(id, nameof(User.IsPhoneConfirmed), isConfirmed); }
        public bool UpdateEmailConfirmation(Guid id, bool isConfirmed) { return updateUserProperty(id, nameof(User.IsEmailConfirmed), isConfirmed); }
        public bool UpdateSecurityStamp(Guid id, string newSecurityStamp) { return updateUserProperty(id, nameof(User.SecurityStamp), newSecurityStamp); }
        public bool UpdateLockoutDate(Guid id, DateTimeOffset? newLockoutDate) { return updateUserProperty(id, nameof(User.LockoutDate), newLockoutDate); }
        public bool UpdateAccessTries(Guid id, int newAccessTries) { return updateUserProperty(id, nameof(User.AccessTries), newAccessTries); }
        public bool UpdateAccountStatus(Guid id, UserAccountStatus accountStatus) { return updateUserProperty(id, nameof(User.AccountStatus), accountStatus); }
        public bool UpdateReputation(Guid id, int newReputation) { return updateUserProperty(id, nameof(User.Reputation), newReputation); }

        private static bool updateEventProperty (Guid id, string propertyName, Object newProperty)
        {
            Event e = new Event { Id = id };

            switch (propertyName)
            {
                case nameof(e.Description):
                    e.Description = (string)newProperty;
                    break;
                case nameof(e.Type):
                    e.Type = (string)newProperty;
                    break;
                case nameof(e.IsEventOpen):
                    e.IsEventOpen = (bool)newProperty;
                    break;
                case nameof(e.EndTime):
                    e.EndTime = (DateTimeOffset?)newProperty;
                    break;
                default:
                    throw new Exception("No propertyName match found");
            }
            using (_context = new QueryContext())
            {
                _context.Events.Attach(e);
                _context.Entry(e).Property<string>(propertyName).IsModified = true;
                _context.SaveChanges();
            }
            return true;
        }
		public bool UpdateDescription(Guid id, string newDescription) { return updateEventProperty(id, nameof(Event.Description), newDescription); }
		public bool UpdateType(Guid id, string newType) { return updateEventProperty(id, nameof(Event.Type), newType); }
        public bool UpdateStatus(Guid id, bool isOpen) { return updateEventProperty(id, nameof(Event.IsEventOpen), isOpen); }
        public bool EndEvent(Guid id) { return updateEventProperty(id, nameof(Event.EndTime), DateTimeOffset.UtcNow); }

        public List<ThinnerUser> GetGuestList(Guid id)
        {
            List<ThinnerUser> guests;
            using (_context = new QueryContext())
            {
                guests = _context.EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Attend).Include(l => l.Self).Select(l => new ThinnerUser(l.Self.Id, l.Self.Name)).ToList();
            }
            return guests;
        }

		public List<EventReport> GetEventReports(Guid id) 
        { 
            throw new NotImplementedException(); 
        }
		public bool ReportEvent(Guid selfId, Guid targetId, EventReportType reportType, string reportDetails) 
        { 
            throw new NotImplementedException(); 
        }

	}
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
