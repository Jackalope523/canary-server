using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Server.Boundaries;
using Shared;
using Repository.Contexts;

namespace Repository
{
    public class QueryStore : IAccountDatabase, IEventDatabase
    {
        public enum StoreMode { Production, Test }

        private StoreMode storeMode;
        private int changeQueueSize;
        private int changesQueued;

        public static IAccountDatabase AccountDatabaseAccess => new QueryStore(StoreMode.Production, 1);
        public static IEventDatabase EventDatabaseAccess => new QueryStore(StoreMode.Production, 1);

        private static QueryContext _context;

        public QueryStore(StoreMode mode, int rate)
        {
            storeMode = mode;
            changeQueueSize = rate;

            if (storeMode == StoreMode.Test) { _context = new TestContext(); }
            else { _context = new TestContext(); }
        }

        private void queueChange()
        {
            changesQueued++;

            if (changesQueued == changeQueueSize)
            {
                changesQueued = 0;

                _context.SaveChanges();
                _context.Dispose();
                
                if (storeMode == StoreMode.Test) { _context = new TestContext(); }
                else { _context = new TestContext(); }
            }
        }

        // Workout Mutual follow search
        public List<ThinnerUser> GetFriends(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Follow); }


        // WTF DOES THIS DO???
        public ThinEvent FindAttendingEvent(Guid id)
		{
			Event @event;
			ThinnerUser host;
			
            @event = _context.EventLinks.Where(e => e.SelfId == id).Include(e => e.Event.Host).Single().Event;
            host = new ThinnerUser(@event.Host.Id, @event.Host.Name);

            queueChange();

            return new ThinEvent(@event.Id, host, @event.Name, @event.Description, @event.Type,
                @event.StartTime, @event.Location.X, @event.Location.Y, @event.EndTime,
				@event.IsEventOpen, @event.GroupMinimum, @event.GroupMaximum);
		}

        // Why need ID here?
        public List<ThinEvent> FindUpcomingEvents(Guid id)
        {
            List<ThinEvent> upcomingEvents;
          
            upcomingEvents = _context.Events.Where(e => e.HostId == id & e.StartTime > DateTimeOffset.UtcNow).
                               Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.Type,
                               e.StartTime, e.Location.Y, e.Location.X, e.EndTime, e.IsEventOpen, e.GroupMinimum, e.GroupMaximum)).ToList();


            queueChange();
            return upcomingEvents;
        }

        // Why need ID here?
        public List<ThinEvent> FindPastEvents(Guid id)
		{
            queueChange();

            List<ThinEvent> pastEvents;
            
            pastEvents = _context.Events.Where(e => e.EndTime.HasValue && e.EndTime < DateTimeOffset.UtcNow).
                               Select(e => new ThinEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Name, e.Description, e.Type,
                               e.StartTime, e.Location.Y, e.Location.X, e.EndTime, e.IsEventOpen, e.GroupMinimum, e.GroupMaximum)).ToList();

            queueChange();
            return pastEvents;
		}

        // Do you really need thin event returned?
        public ThinEvent CreateEvent(Guid hostId, string name, string description, string eventType, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum)
        {
            queueChange();

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

            _context.Events.Add(toCreate);

            AddUserToEvent(hostId, toCreate.Id);

            queueChange();
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

            _context.Users.Add(toCreate);

            queueChange();
            return true;
        }
        public bool DeleteUser(Guid id)
        {

            _context.Users.Remove(new User { Id = id });

            queueChange();
            return true;
        }

        

        public ThinEvent FindEvent(Guid id)
        {
            

            ThinEvent @event;          
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

            queueChange();
            return @event;
        }
        public List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance)
        {
            List<ThinnerEvent> closestEvents;
            Point userLocation = new Point(longitude, latitude);
           
            closestEvents = _context.Events.Where(e => e.Location.Distance(userLocation) <= distance && !e.EndTime.HasValue).
                                Select(e => new ThinnerEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Type, e.Location.Y, e.Location.X)).ToList();

            queueChange();
            return closestEvents;
        }

        public List<ThinnerUser> GetGuestList(Guid id)
        {
            queueChange();

            List<ThinnerUser> guests;
            
            guests = _context.EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Attend).Select(l => new ThinnerUser(l.Self.Id, l.Self.Name)).ToList();

            return guests;
        }

        private ThinUser FindUserBy(Func<User, bool> predicate)
        {
            ThinUser user;
            int numFollowers;

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

            queueChange();
            return user with { NumberOfFollowers = numFollowers };
        }
        public ThinUser FindUserById(Guid id) { return FindUserBy(u => u.Id == id); }
        public ThinUser FindUserByPhoneNumber(string phoneNumber) { return FindUserBy(u => u.PhoneNumber == phoneNumber); }
        public ThinUser FindUserByEmail(string email) { return FindUserBy(u => u.NormalizedEmail == email); }

        private bool addLinkOperation(Link link)
        {
            _context.Links.Add(link);
            queueChange();
            return true;

        }
        private bool removeLinkOperation(UserLink link)
        {
            _context.UserLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete();

            //queueChange();
            return true;
        }
        private bool removeLinkOperation(EventLink link)
        {
            _context.EventLinks.Where(l => l.SelfId == link.SelfId && l.EventId == link.EventId && l.Type == link.Type).ExecuteDelete();
            //queueChange();
            return true;
        }

        public bool FollowUser(Guid selfId, Guid targetId) { return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public bool UnfollowUser(Guid selfId, Guid targetId) { return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public bool BlockUser(Guid selfId, Guid targetId) { return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }
        public bool RateUser(Guid selfId, Guid targetId, UserRating rating) 
        {
            

            if (rating == UserRating.Positive) return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.RateUp });
            else return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.RateDown });
        }

        public bool AddUserToEvent(Guid userId, Guid eventId) { return addLinkOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attend }); }
        public bool RemoveUserFromEvent(Guid userId, Guid eventId) { return removeLinkOperation(new EventLink { SelfId = userId, EventId = eventId, Type = EventLink.EventLinkType.Attend }); }

        private bool updateUserProperty(Guid id, string propertyName, Object newProperty)
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

            _context.Users.Attach(u);
            _context.Entry(u).Property(propertyName).IsModified = true;

            queueChange();
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

        private bool updateEventProperty (Guid id, string propertyName, Object newProperty)
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

            _context.Events.Attach(e);
            _context.Entry(e).Property<string>(propertyName).IsModified = true;

            queueChange();
            return true;
        }
		public bool UpdateDescription(Guid id, string newDescription) { return updateEventProperty(id, nameof(Event.Description), newDescription); }
		public bool UpdateType(Guid id, string newType) { return updateEventProperty(id, nameof(Event.Type), newType); }
        public bool UpdateStatus(Guid id, bool isOpen) { return updateEventProperty(id, nameof(Event.IsEventOpen), isOpen); }
        public bool EndEvent(Guid id) { return updateEventProperty(id, nameof(Event.EndTime), DateTimeOffset.UtcNow); }

        private List<ThinnerUser> getLinkedUsers(Guid id, UserLink.UserLinkType type)
        {
            List<ThinnerUser> users;
          
            users = _context.UserLinks.Where(l => l.SelfId == id && l.Type == type).Select(l => new ThinnerUser(l.Other.Id, l.Other.Name)).ToList();

            queueChange();
            return users;
        }  
        public List<ThinnerUser> GetFollowedUsers(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Follow); }
        public List<ThinnerUser> GetBlockedUsers(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Block); }
     
        public (int Positive, int Negative) GetUserRatings(Guid id)
        {
            List<UserLink.UserLinkType> ratings;
            ratings = _context.UserLinks.Where(l => l.OtherId == id && (l.Type == UserLink.UserLinkType.RateUp || l.Type == UserLink.UserLinkType.RateDown)).Select(l => l.Type).ToList();

            int up = 0;
            int down = 0;
            foreach (UserLink.UserLinkType rating in ratings)
            {
                if (rating == UserLink.UserLinkType.RateUp) up++;
                else down--;
            }
            queueChange();
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
