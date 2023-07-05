using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using PhoneNumbers;
using Repository.Entities;
using Repository.Sentries;
using Server.Boundaries;
using Shared;
using SQLitePCL;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class QueryStore : IAccountDatabase, IEventDatabase
    {     
        public static IAccountDatabase AccountDatabaseAccess => new QueryStore(new TestSentry());
        public static IEventDatabase EventDatabaseAccess => new QueryStore(new TestSentry());

        Sentry storeSentry;

        public QueryStore(Sentry sentry)
        {
            storeSentry = sentry;
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
                NormalizedEmail = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                AccountStatus = UserAccountStatus.active,
            };

            storeSentry.GetContext().Users.Add(toCreate);
            return true;
        }
        public bool DeleteUser(Guid id)
        {
            storeSentry.GetContext().Users.Remove(new User { Id = id });           
            return true;
        }

      

        public ThinEvent FindEvent(Guid id)
        {
            ThinEvent @event;
            @event = storeSentry.GetContext().Events.Where(e => e.Id == id).Select(e => new ThinEvent
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

            return @event;
        }
        public List<ThinnerEvent> FindEvents(double latitude, double longitude, double distance)
        {
            List<ThinnerEvent> closestEvents;
            Point userLocation = new Point(longitude, latitude);

            closestEvents = storeSentry.GetContext().Events.Where(e => e.Location.Distance(userLocation) <= distance && !e.EndTime.HasValue).
                                Select(e => new ThinnerEvent(e.Id, new ThinnerUser(e.Host.Id, e.Host.Name), e.Type, e.Location.Y, e.Location.X)).ToList();

            return closestEvents;
        }

        public List<ThinnerUser> GetGuestList(Guid id)
        {
            List<ThinnerUser> guests;

            guests = storeSentry.GetContext().EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Attend).Select(l => new ThinnerUser(l.Self.Id, l.Self.Name)).ToList();

            return guests;
        }

        private ThinUser FindUserBy(Func<User, bool> predicate)
        {
            ThinUser user;
            int numFollowers;

            user = storeSentry.GetContext().Users.Where(predicate).Select(u => new ThinUser
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

            numFollowers = storeSentry.GetContext().UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).Count();

            return user with { NumberOfFollowers = numFollowers };
        }
        public ThinUser FindUserById(Guid id) { return FindUserBy(u => u.Id == id); }
        public ThinUser FindUserByPhoneNumber(string phoneNumber) { return FindUserBy(u => u.PhoneNumber == phoneNumber); }
        public ThinUser FindUserByEmail(string email) { return FindUserBy(u => u.NormalizedEmail == email); }

        private bool addLinkOperation(Link link)
        {
            storeSentry.GetContext().Links.Add(link);
            return true;

        }
        private bool removeLinkOperation(UserLink link)
        {
            storeSentry.GetContext().UserLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete();

            //queueChange();
            return true;
        }
        private bool removeLinkOperation(EventLink link)
        {
            storeSentry.GetContext().EventLinks.Where(l => l.SelfId == link.SelfId && l.EventId == link.EventId && l.Type == link.Type).ExecuteDelete();
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

            storeSentry.GetContext().Users.Attach(u);
            storeSentry.GetContext().Entry(u).Property(propertyName).IsModified = true;

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

        private bool updateEventProperty(Guid id, string propertyName, Object newProperty)
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

            storeSentry.GetContext().Events.Attach(e);
            storeSentry.GetContext().Entry(e).Property(propertyName).IsModified = true;
          
            return true;
        }
        public bool UpdateDescription(Guid id, string newDescription) { return updateEventProperty(id, nameof(Event.Description), newDescription); }
        public bool UpdateType(Guid id, string newType) { return updateEventProperty(id, nameof(Event.Type), newType); }
        public bool UpdateStatus(Guid id, bool isOpen) { return updateEventProperty(id, nameof(Event.IsEventOpen), isOpen); }
        public bool EndEvent(Guid id) { return updateEventProperty(id, nameof(Event.EndTime), DateTimeOffset.UtcNow); }

        private List<ThinnerUser> getLinkedUsers(Guid id, UserLink.UserLinkType type)
        {
            List<ThinnerUser> users;

            users = storeSentry.GetContext().UserLinks.Where(l => l.SelfId == id && l.Type == type).Select(l => new ThinnerUser(l.Other.Id, l.Other.Name)).ToList();

           
            return users;
        }
        public List<ThinnerUser> GetFollowedUsers(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Follow); }
        public List<ThinnerUser> GetBlockedUsers(Guid id) { return getLinkedUsers(id, UserLink.UserLinkType.Block); }

        public (int Positive, int Negative) GetUserRatings(Guid id)
        {
            List<UserLink.UserLinkType> ratings;
            ratings = storeSentry.GetContext().UserLinks.Where(l => l.OtherId == id && (l.Type == UserLink.UserLinkType.RateUp || l.Type == UserLink.UserLinkType.RateDown)).Select(l => l.Type).ToList();

            int up = 0;
            int down = 0;
            foreach (UserLink.UserLinkType rating in ratings)
            {
                if (rating == UserLink.UserLinkType.RateUp) up++;
                else down--;
            }
            
            return (up, down);
        }
        //_________________________________________________________________________________________________________________________
        public (List<UserReport>, List<EventReport>) GetReports(Guid id)
        {
            throw new NotImplementedException();
        }

        public (List<UserReport>, List<EventReport>) GetReportsByUser(Guid id)
        {
            throw new NotImplementedException();
        } 

        public List<EventReport> GetEventReports(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool ReportUser(Guid selfId, Guid targetId, UserReportType reportType, string reportDetails)
        {
            Report toCreate = new Report
            {
                SelfId = selfId,
                OtherId = targetId,
                EventId = eventId,
                Type = Report.ToReportType(reportType),
                Notes = reportDetails
            };

            storeSentry.GetContext().Reports.Add(toCreate);
            return true;
        }

        public bool ReportEvent(Guid userId, Guid eventId, Guid HostId, EventReportType reportType, string reportDetails)
        {
            Report toCreate = new Report
            {
                SelfId = userId,
                OtherId = HostId,
                EventId = eventId,
                Type = Report.ToReportType(reportType),
                Notes = reportDetails
            };

            storeSentry.GetContext().Reports.Add(toCreate);
            return true;
        }

        //_________________________________________________________________________________________________________________________

        private List<ThinEvent> FindEventsBy(Func<EventLink, bool> predicate )
        {     
            List<Guid> guids= new List<Guid>();
            guids = storeSentry.GetContext().EventLinks.Where(predicate).Select(l => l.EventId).ToList();

            List<ThinEvent> events;
            events = storeSentry.GetContext().Events.Where(e => guids.Contains(e.Id)).Select(e => new ThinEvent
               (
                   e.Id,
                   new ThinnerUser(e.HostId, e.Host.Name),
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
               )).ToList();

            return events;
        }
        public ThinEvent FindCurrentEvent(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Attend).Single(); }
        public List<ThinEvent> FindUpcomingEvents(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Watch); }
        public List<ThinEvent> FindPastEvents(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Left); }
        

        public List<ThinnerUser> GetFriends(Guid id)
        {
            throw new NotImplementedException();
        }

        public ThinEvent CreateEvent(Guid hostId, string name, string description, string eventType, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum)
        {
            /*
            Event toCreate = new Event 
            { 
                HostId = hostId, 
                Name = name, 
                Description = description, 
                Type = eventType, 
                StartTime = startTime, 
                Location = new Point(longitude, latitude), 
                GroupMinimum = groupMinimum, 
                GroupMaximum = groupMaximum 
            };

            storeSentry.GetContext().Events.Add(toCreate);


            return true;
            */
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
