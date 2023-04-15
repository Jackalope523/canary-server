using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using PhoneNumbers;
using Server.Boundaries;
using Shared;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataAccess
{
    public class QueryStore : IAccountDatabase, IEventDatabase
    {
        public static IAccountDatabase AccountDatabaseAccess => new QueryStore();
        public static IEventDatabase EventDatabaseAccess => new QueryStore();

        private static QueryContext _context = new QueryContext();




        // Workout Mutual follow search
        public List<ThinnerUser> GetFriends(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Follow); }


        // WTF DOES THIS DO???
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

        // Why need ID here?
        public List<ThinEvent> FindUpcomingEvents(Guid id)
        {
            List<ThinEvent> upcomingEvents;
            using (_context = new QueryContext())
            { 
                upcomingEvents = _context.Events.Where(e => e.HostId == id &e.StartTime > DateTimeOffset.UtcNow).
								Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.Type,
                                e.StartTime, e.Location.Y, e.Location.X, e.EndTime, e.IsEventOpen, e.GroupMinimum, e.GroupMaximum)).ToList();
            }
            return upcomingEvents;
        }

        // Why need ID here?
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

        // Do you really need thin event returned?
        public ThinEvent CreateEvent(Guid hostId, string name, string description, string eventType, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum)
        {
            Event toCreate = new Event
            {
                HostId = hostId,
                Name = name,
                Type = eventType,
                StartTime = startTime,
                Location = new Point(longitude, latitude),
                IsEventOpen = true,
                GroupMinimum = groupMinimum,
                GroupMaximum = groupMaximum
            };

            using (_context = new QueryContext())
            {
                _context.Events.Add(toCreate);
                _context.SaveChanges();

                AddUserToEvent(hostId, toCreate.Id);
            }

            return toCreate.ToThinEvent();
        }



        //************************************************
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
                NormalizedEmail = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                AccountStatus = UserAccountStatus.active,
            };

            using (_context = new QueryContext())
            {
                _context.Users.Add(toCreate);
                _context.SaveChanges();
            }

            return true;
        }
        public bool DeleteUser(Guid id)
        {
            using (_context = new QueryContext())
            {
                _context.Users.Remove(new User { Id = id });
                _context.SaveChanges();
            }

            return true;
        }

        

        public ThinEvent FindEvent(Guid id)
        {
            ThinEvent @event;
            using (_context = new QueryContext())
            {
                @event = _context.Events.Where(e => e.Id == id).Select(e => new ThinEvent
                (
                    e.Id, 
                    new ThinnerUser(e.Host.Id, e.Host.Name), 
                    e.Name, 
                    e.Description, 
                    e.Type,
                    e.StartTime, 
                    e.Location.Y, 
                    e.Location.X, 
                    e.EndTime,
                    e.IsEventOpen, 
                    e.GroupMinimum, 
                    e.GroupMaximum
                )).Single();
            }

            return @event;
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

        public List<ThinnerUser> GetGuestList(Guid id)
        {
            List<ThinnerUser> guests;
            using (_context = new QueryContext())
            {
                guests = _context.EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Attend).Select(l => new ThinnerUser(l.Self.Id, l.Self.Name)).ToList();
            }
            return guests;
        }

        private ThinUser FindUserBy(Func<User, bool> predicate)
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
            return user with { NumberOfFollowers = numFollowers };
        }
        public ThinUser FindUserById(Guid id) { return FindUserBy(u => u.Id == id); }
        public ThinUser FindUserByPhoneNumber(string phoneNumber) { return FindUserBy(u => u.PhoneNumber == phoneNumber); }
        public ThinUser FindUserByEmail(string email) { return FindUserBy(u => u.NormalizedEmail == email); }

        // Come back here once exclusivity decided. 
        private static bool linkOperation(Link link, bool addOperation)
        {
            using (_context = new QueryContext())
            {
                if (addOperation) _context.Links.Add(link);
                else _context.Links.Remove(link);
                _context.SaveChanges();
            }
            return true;
        }
        public bool FollowUser(Guid selfId, Guid targetId) { return linkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }, true); }
        public bool UnfollowUser(Guid selfId, Guid targetId) { return linkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }, false); }
        public bool BlockUser(Guid selfId, Guid targetId) { return linkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }, true); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return linkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }, false); }
        public bool RateUser(Guid selfId, Guid targetId, UserRating rating) 
        {
            if (rating == UserRating.Positive) return linkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.RateUp }, true);
            else return linkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.RateDown }, true);
        }

        public bool AddUserToEvent(Guid userId, Guid eventId) { return linkOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attend }, true); }
        public bool RemoveUserFromEvent(Guid userId, Guid eventId) { return linkOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attend }, false); }

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
                case nameof(u.NormalizedEmail):
                    u.NormalizedEmail = (string)newProperty;
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
        public bool UpdateNormalisedEmail(Guid id, string newNormalisedEmail) { return updateUserProperty(id, nameof(User.NormalizedEmail), newNormalisedEmail); }
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

        private static List<ThinnerUser> getLinkedUsers(Guid id, UserLink.UserLinkType type)
        {
            List<ThinnerUser> users;
            using (_context = new QueryContext())
            {
                users = _context.UserLinks.Where(l => l.SelfId == id && l.Type == type).Select(l => new ThinnerUser(l.Other.Id, l.Other.Name)).ToList();
            }
            return users;
        }  
        public List<ThinnerUser> GetFollowedUsers(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Follow); }
        public List<ThinnerUser> GetBlockedUsers(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Block); }
     
        public (int Positive, int Negative) GetUserRatings(Guid id)
        {
            List<UserLink.UserLinkType> ratings;
            using (_context = new QueryContext())
            {
                ratings = _context.UserLinks.Where(l => l.OtherId == id && (l.Type == UserLink.UserLinkType.RateUp || l.Type == UserLink.UserLinkType.RateDown)).Select(l => l.Type).ToList();
            }

            int up = 0;
            int down = 0;
            foreach (UserLink.UserLinkType rating in ratings)
            {
                if (rating == UserLink.UserLinkType.RateUp) up++;
                else down--;
            }
            return (up, down);
        }

        public (List<UserReport>, List<EventReport>) GetReports(Guid id)
        {
            throw new NotImplementedException();
        }

        public (List<UserReport>, List<EventReport>) GetReportsByUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool ReportUser(Guid selfId, Guid targetId, UserReportType reportType, string reportDetails)
        {
            throw new NotImplementedException();
        }

        public List<EventReport> GetEventReports(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool ReportEvent(Guid userId, Guid eventId, EventReportType reportType, string reportDetails)
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
