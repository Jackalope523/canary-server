using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using Repository.Entities;
using Repository.Sentries;
using Server.Boundaries;
using Shared;
using System;
using System.Numerics;
using static Repository.Entities.Report;

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

        public bool CreateUser(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth, Character character)
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
                Extroversion = character.Extraversion,
                Athleticisme = character.Athleticism,
                Openness = character.Openness,
                Chaos = character.Chaoticness,
                Competitiveness = character.Competitiveness,
                Industriousness = character.Industriousness,
                NightOwl = character.NightOwl,
            };

            storeSentry.GetContext().Users.Add(toCreate);
            return true;
        }
        public bool DeleteUser(Guid id)
        {
            storeSentry.GetContext().Users.Remove(new User { Id = id });           
            return true;
        }

        public EventShard CreateEvent(Guid hostId, string name, string description, string eventType, DateTimeOffset startTime, double latitude, double longitude, int groupMinimum, int groupMaximum, Character character)
        {
            Event toCreate = new Event
            {
                HostId = hostId,
                Name = name,
                Description = description,
                Type = eventType,
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

            storeSentry.GetContext().Events.Add(toCreate);


            return new ThinEvent
                (
                   toCreate.Id,
                   new UserSilhouette(toCreate.Host.Id, toCreate.Host.Name),
                   toCreate.Name,
                   toCreate.Description,
                   toCreate.Type,
                   toCreate.StartTime,
                   toCreate.Location.Y,
                   toCreate.Location.X,
                   toCreate.EndTime,
                   toCreate.IsEventOpen,
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


        public EventShard FindEvent(Guid id)
        {
            EventShard @event;
            @event = storeSentry.GetContext().Events.Where(e => e.Id == id).Select(e => new ThinEvent
               (
                   e.Id,
                   new UserSilhouette(e.Host.Id, e.Host.Name),
                   e.Name,
                   e.Description,
                   e.Type,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.IsEventOpen,
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
               )).Single();

            return @event;
        }
        public List<EventThinSlice> FindEvents(double latitude, double longitude, double distance)
        {
            List<EventThinSlice> closestEvents;
            Point userLocation = new Point(longitude, latitude);

            closestEvents = storeSentry.GetContext().Events.Where(e => e.Location.Distance(userLocation) <= distance && !e.EndTime.HasValue).
                                Select(e => new ThinnerEvent(e.Id, new UserSilhouette(e.Host.Id, e.Host.Name), e.Type, e.Location.Y, e.Location.X)).ToList();

            return closestEvents;
        }

        public List<UserSilhouette> GetGuestList(Guid id)
        {
            List<UserSilhouette> guests;

            guests = storeSentry.GetContext().EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Attend).Select(l => new UserSilhouette(l.Self.Id, l.Self.Name)).ToList();

            return guests;
        }

        private UserShard FindUserBy(Func<User, bool> predicate)
        {
            UserShard user;
            int numFollowers;

            user = storeSentry.GetContext().Users.Where(predicate).Select(u => new UserShard
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
                   -1,
                   new Character(
                   u.Extroversion,
                   u.Athleticisme,
                   u.Chaos,
                   u.Competitiveness,
                   u.Industriousness,
                   u.NightOwl,
                   u.Openness)
               )).Single();

            numFollowers = storeSentry.GetContext().UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).Count();

            return user with { NumberOfFollowers = numFollowers };
        }
        public UserShard FindUserById(Guid id) { return FindUserBy(u => u.Id == id); }
        public UserShard FindUserByPhoneNumber(string phoneNumber) { return FindUserBy(u => u.PhoneNumber == phoneNumber); }
        public UserShard FindUserByEmail(string email) { return FindUserBy(u => u.NormalizedEmail == email); }

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
        private bool removeLinkOperation(PostLink link)
        {
            storeSentry.GetContext().PostLinks.Where(l => l.SelfId == link.SelfId && l.PostId == link.PostId).ExecuteDelete();
            //queueChange();
            return true;
        }

        public bool FollowUser(Guid selfId, Guid targetId) { return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public bool UnfollowUser(Guid selfId, Guid targetId) { return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public bool BlockUser(Guid selfId, Guid targetId) { return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }
        public bool RateUser(Guid selfId, Guid targetId, UserRating rating)
        {
            UserLink.UserLinkType type;
            if (rating.Equals(UserRating.Positive)) type = UserLink.UserLinkType.RateUp;
            else type = UserLink.UserLinkType.RateDown;

            return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = type });
        }
        public bool RatePost(Guid postId, Guid voterId, UserRating rating)
        {
            PostLink.PostLinkType type;
            if (rating.Equals(UserRating.Positive)) type = PostLink.PostLinkType.RateUp;
            else type = PostLink.PostLinkType.RateDown;

            return addLinkOperation(new PostLink { SelfId = voterId, PostId = postId, Type = type });
        }
        public bool RemoveUserRating(Guid selfId, Guid targetId)
        {
            return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId });
        }
        public bool RemovePostRating(Guid postId, Guid voterId)
        {
            return removeLinkOperation(new PostLink { SelfId = voterId, PostId = postId });
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
        public bool UpdateHaunt(Guid id, double latitude, double longitude, double radius, int weight)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRecentLocation(Guid id, double latitude, double longitude, double radius)
        {
            throw new NotImplementedException();
        }

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
       
        private List<UserSilhouette> getUsersBy(Func<UserLink,bool> predicate)
        {
            List<UserSilhouette> users;

            users = storeSentry.GetContext().UserLinks.Where(predicate).Select(l => new UserSilhouette(l.Other.Id, l.Other.Name)).ToList();
            return users;
        }
        public List<UserSilhouette> GetFollowedUsers(Guid id) { return getUsersBy(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow); }
        public List<UserSilhouette> GetBlockedUsers(Guid id) { return getUsersBy(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Block); }
        public List<UserSilhouette> GetFriends(Guid id)
        {
            List<UserSilhouette> following = getUsersBy(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow);
            List<UserSilhouette> followingMe = getUsersBy(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow);     
            return following.Intersect(followingMe).ToList();
        }

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

        private List<Report> getReports(Func<Report,bool> predicate)
        {
            return storeSentry.GetContext().Reports.Where(r => predicate(r)).ToList();
        }
        public List<EventReport> GetReportsAboutEvent(Guid id)
        {
            List<Report> reports = getReports(r => r.EventId == id);
            List<EventReport> toReturn = new List<EventReport>();

            foreach (Report report in reports)
            {
                toReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));
            }

            return toReturn;
        }
        public (List<UserReport>, List<EventReport>) GetReportsAboutUser(Guid id)
        {
            List<Report> userReports = getReports(r => r.OtherId == id);
            List<Report> eventReports = getReports(r => r.OtherId == id && r.OtherId == r.Event.HostId);
            List<UserReport> userReportsToReturn = new List<UserReport>();
            List<EventReport> eventReportsToReturn = new List<EventReport>();

            foreach (Report report in userReports)
            {
                userReportsToReturn.Add(new UserReport(report.Id, report.SelfId, report.OtherId, report.FilingDate, Report.ToUserReportType(report.Type), report.Notes));
            }
            foreach (Report report in eventReports)
            {
                eventReportsToReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));

            }

            return (userReportsToReturn, eventReportsToReturn);
        }
        public (List<UserReport>, List<EventReport>) GetReportsByUser(Guid id)
        {
            List<Report> reports = getReports(r => r.SelfId == id);
            List<UserReport> userReportsToReturn = new List<UserReport>();
            List<EventReport> eventReportsToReturn = new List<EventReport>();

            foreach (Report report in reports)
            {
                if (report.OtherId != report.Event.HostId)
                {
                    userReportsToReturn.Add(new UserReport(report.Id, report.SelfId, report.OtherId, report.FilingDate, Report.ToUserReportType(report.Type), report.Notes));
                }
                else
                {
                    eventReportsToReturn.Add(new EventReport(report.Id, report.SelfId, report.EventId, report.OtherId, report.FilingDate, Report.ToEventReportType(report.Type), report.Notes));
                }
                
            }       
            return (userReportsToReturn, eventReportsToReturn);
        } 
       
        private bool CreateReport(Guid userId, Guid eventId, Guid HostId, ReportType reportType,  DateTimeOffset filingDate, string reportDetails)
        {
            Report toCreate = new Report
            {
                SelfId = userId,
                OtherId = HostId,
                EventId = eventId,
                Type = reportType,
                FilingDate = filingDate,
                Notes = reportDetails
            };

            storeSentry.GetContext().Reports.Add(toCreate);
            return true;
        }
        public bool ReportUser(Guid selfId, Guid eventId, Guid targetId, UserReportType reportType, string reportDetails) 
        {
            return CreateReport(selfId, eventId, targetId, Report.ToReportType(reportType), DateTimeOffset.Now, reportDetails);
        }
        public bool ReportEvent(Guid userId, Guid eventId, Guid HostId, EventReportType reportType, string reportDetails)
        {
            
            return CreateReport(userId, eventId, HostId, Report.ToReportType(reportType), DateTimeOffset.Now, reportDetails);
        }

        private List<EventShard> FindEventsBy(Func<EventLink, bool> predicate )
        {
            List<Guid> guids= new List<Guid>();
            guids = storeSentry.GetContext().EventLinks.Where(predicate).Select(l => l.EventId).ToList();

            List<EventShard> events;
            events = storeSentry.GetContext().Events.Where(e => guids.Contains(e.Id)).Select(e => new ThinEvent
               (
                   e.Id,
                   new UserSilhouette(e.HostId, e.Host.Name),
                   e.Name,
                   e.Description,
                   e.Type,
                   e.StartTime,
                   e.Location.Y,
                   e.Location.X,
                   e.EndTime,
                   e.IsEventOpen,
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
               )).ToList();

            return events;
        }
        public EventShard FindCurrentEvent(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Attend).Single(); }
        public List<EventShard> FindUpcomingEvents(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Watch); }
        public List<EventShard> FindPastEvents(Guid id) { return FindEventsBy(l => l.SelfId == id && l.Type == EventLink.EventLinkType.Left); }  

        EventPost IEventDatabase.AddPost(Guid eventId, Guid posterId, DateTimeOffset timePosted, string imageURL)
        {
            Post toAdd = new Post { EventId = eventId, OwnerId = posterId, PostedAt = timePosted, PhotoURL = imageURL };
            storeSentry.GetContext().Posts.Add(toAdd);
            return new EventPost(toAdd.Id, toAdd.EventId, toAdd.OwnerId, toAdd.PostedAt, toAdd.PhotoURL, new (0, 0));
        }

        public bool RemovePost(Guid postId)
        {
            storeSentry.GetContext().Posts.Remove(new Post { Id = postId });
            return true;
        }

        private int countRatings(Guid id, PostLink.PostLinkType type)
        {
            return storeSentry.GetContext().PostLinks.Where(l => l.PostId == id && l.Type == type).Count();
        }
        public EventPost GetPost(Guid id)
        {
            int Ups = countRatings(id, PostLink.PostLinkType.RateUp);
            int Downs = countRatings(id, PostLink.PostLinkType.RateDown);

            return storeSentry.GetContext().
                Posts.
                Where(p => p.Id == id).
                Select(p => new EventPost(p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, new (Ups, Downs))).Single();
        }

        public List<EventPost> GetPostsForEvent(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUserCharacter(Guid id, int extraversion, int athleticism, int chaoticness, int competitiveness, int industriousness, int nightOwl, int openness)
        {
            User toUpdate = new User
            {
                Id = id,
                Extroversion = extraversion,
                Athleticisme = athleticism,
                Openness = openness,
                Chaos = chaoticness,
                Competitiveness = competitiveness,
                Industriousness = industriousness,
                NightOwl = nightOwl
            };
            storeSentry.GetContext().Users.Update(toUpdate);

            return true;
        }

       
        public (double Latitude, double Longitude, double Radius, int Stability) GetUserHaunt(Guid id)
        {
            var result = storeSentry.GetContext().Users.Where(u => u.Id == id).Select(u => new { u.Haunt.Y, u.Haunt.X, u.HauntRadius, u.HauntWheight }).Single();
            return (result.Y, result.X, result.HauntRadius, result.HauntWheight);
        }

      

        public (double Latitude, double Longitude, double Radius) GetRecentUserLocation(Guid id)
        {
            var result = storeSentry.GetContext().Users.Where(u => u.Id == id).Select(u => new { u.CurrentLocation.Y, u.CurrentLocation.X, u.CurrentRadius }).Single();
            return (result.Y, result.X, result.CurrentRadius);
        }

        public List<EventPost> GetPostsByUser(Guid id)
        {

            return storeSentry.GetContext().Posts.Where(p => p.OwnerId == id).
                Join(
                storeSentry.GetContext().PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() }),
                p => p.Id,
                l => l.PostId,
                (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
                ).
                Join(
                storeSentry.GetContext().PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() }),
                p => p.Id,
                l => l.PostId,
                (a, b) => new EventPost(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
                )).ToList();
        }

        public List<EventPost> GenerateFeedForUser(Guid id, DateTimeOffset depthCharge, List<Guid> exclusionList)
        {           
            // Get List of Friends.
            List<Guid> following = storeSentry.GetContext().UserLinks.Where(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.OtherId).ToList();
            List<Guid> followingMe = storeSentry.GetContext().UserLinks.Where(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow).Select(l => l.SelfId).ToList();
            List<Guid> friends =  following.Intersect(followingMe).ToList();

            // Get unseen posts by friends from certain depth.
            List<EventPost> friendPosts = storeSentry.GetContext().Posts.Where(p => friends.Contains(p.OwnerId) && !exclusionList.Contains(p.EventId) && p.PostedAt > depthCharge && p.PostedAt < DateTimeOffset.UtcNow).
               Join(
               storeSentry.GetContext().PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() }),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
               ).
               Join(
               storeSentry.GetContext().PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() }),
               p => p.Id,
               l => l.PostId,
               (a, b) => new EventPost(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
               )).ToList();

            // Compile unique list if events spanned by friend posts and a list of already loaded posts. 
            List<Guid> sitesToBeExplored = new List<Guid>();
            List<Guid> previouslyExtractedPosts = new List<Guid>();
            foreach (EventPost p in friendPosts)
            {
                if (!sitesToBeExplored.Contains(p.EventId)) sitesToBeExplored.Add(p.EventId);
                previouslyExtractedPosts.Add(p.Id);
            }

            // Get remaining friend posts from same events as others even if outside time range. 
            List<EventPost> nettedPosts = storeSentry.GetContext().Posts.Where(p => friends.Contains(p.OwnerId) && !previouslyExtractedPosts.Contains(p.Id) && sitesToBeExplored.Contains(p.EventId)).
               Join(
               storeSentry.GetContext().PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateUp).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateUps = l.Count() }),
               p => p.Id,
               l => l.PostId,
               (p, l) => new { p.Id, p.EventId, p.OwnerId, p.PostedAt, p.PhotoURL, l.RateUps }
               ).
               Join(
               storeSentry.GetContext().PostLinks.Where(l => l.Type == PostLink.PostLinkType.RateDown).GroupBy(l => l.PostId).Select(l => new { PostId = l.Key, RateDowns = l.Count() }),
               p => p.Id,
               l => l.PostId,
               (a, b) => new EventPost(a.Id, a.EventId, a.OwnerId, a.PostedAt, a.PhotoURL, new(a.RateUps, b.RateDowns)
               )).ToList();

            return friendPosts.Concat(nettedPosts).ToList();
        }
    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
