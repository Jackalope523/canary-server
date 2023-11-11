using Microsoft.EntityFrameworkCore;
using Core.Boundaries;

namespace Repository
{
    public abstract class QueryStore
    {

        protected Sentry storeSentry;

        public QueryStore(Sentry sentry)
        {
            storeSentry = sentry;
        }

        // UTIL
        protected UserShard FindUserBy(Func<User, bool> predicate)
        {
            UserShard user;
            int numFollowers;

            user = storeSentry.ExecuteRead(ctx => ctx.Users.Where(predicate).Select(u => new UserShard
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
               )).Single());

            numFollowers = storeSentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.OtherId == user.Id && l.Type == UserLink.UserLinkType.Follow).Count());

            return user with { NumberOfFollowers = numFollowers };
        }

        protected bool addLinkOperation(Link link)
        {
            storeSentry.ExecuteWrite(ctx => ctx.Links.Add(link));
            return true;

        }
        protected bool removeLinkOperation(UserLink link)
        {
            storeSentry.ExecuteWrite(ctx => ctx.UserLinks.Where(l => l.SelfId == link.SelfId && l.OtherId == link.OtherId && l.Type == link.Type).ExecuteDelete());
            return true;
        }
        protected bool removeLinkOperation(EventLink link)
        {
            storeSentry.ExecuteWrite(ctx => ctx.EventLinks.Where(l => l.SelfId == link.SelfId && l.EventId == link.EventId && l.Type == link.Type).ExecuteDelete());
            return true;
        }
        protected bool removeLinkOperation(PostLink link)
        {
            storeSentry.ExecuteWrite(ctx => ctx.PostLinks.Where(l => l.SelfId == link.SelfId && l.PostId == link.PostId && l.Type == link.Type).ExecuteDelete());
            return true;
        }
       
        protected List<UserSilhouette> getUsersBy(Func<UserLink, bool> predicate)
        {
            List<UserSilhouette> users;

            users = storeSentry.ExecuteRead(ctx => ctx.UserLinks.Where(predicate).Select(l => new UserSilhouette(l.Other.Id, l.Other.Name)).ToList());
            return users;
        }

        protected List<Report> getReports(Func<Report, bool> predicate)
        {
            return storeSentry.ExecuteRead(ctx => ctx.Reports.Where(r => predicate(r)).ToList());
        }

        protected bool CreateReport(Guid userId, Guid eventId, Guid HostId, Report.ReportType reportType, DateTimeOffset filingDate, string reportDetails)
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

            storeSentry.ExecuteWrite(ctx => ctx.Reports.Add(toCreate));
            return true;
        }

        protected List<EventShard> FindEventsBy(Func<EventLink, bool> predicate)
        {
            List<Guid> guids = new List<Guid>();
            guids = storeSentry.ExecuteRead(ctx => ctx.EventLinks.Where(predicate).Select(l => l.EventId).ToList());

            List<EventShard> events;
            events = storeSentry.ExecuteRead(ctx=> ctx.Events.Where(e => guids.Contains(e.Id)).Select(e => new EventShard
               (
                   e.Id,
                   new UserSilhouette(e.HostId, e.Host.Name),
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
               )).ToList());

            return events;
        }

        protected int countRatings(Guid id, PostLink.PostLinkType type)
        {
            return storeSentry.ExecuteRead(ctx => ctx.PostLinks.Where(l => l.PostId == id && l.Type == type).Count());
        }
    }
}


/*
    _.+._
  (^\/^\/^)
   \D*O*D/
   {_____}
           */
