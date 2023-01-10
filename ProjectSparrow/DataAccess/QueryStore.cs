using Server.Boundaries;
using DataAccess.Entities;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared;

namespace DataAccess
{ 
    internal class QueryStore : IAccountDatabase, IEventDatabase
    {
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

        Func<Entity, EntityEntry> addUserLink = l => _context.UserLinks.Add((UserLink)l);
        Func<Entity, EntityEntry> removeUserLink = l => _context.UserLinks.Remove((UserLink)l);
        public bool FollowUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, addUserLink); }      
        public bool UnfollowUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, removeUserLink); } 
        public bool BlockUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, addUserLink); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return EntityOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, removeUserLink); }          

        private static List<ThinnerUser> GetCollectionOfUsers(Guid id, UserLink.UserLinkType type)
        {
            List<ThinnerUser> blockedUsers;
            using (_context = new QueryContext())
            {
                blockedUsers = _context.UserLinks.Where(l => l.SelfId == id && l.Type == type).Include(l => l.Other).Select(l => new ThinnerUser(l.Other.Id, l.Other.Name)).ToList();
            }
            return blockedUsers;
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
            return new ThinUser(user.Id, user.PhoneNumber, user.Name, user.DateOfBirth, user.Reputation, numFollowers);
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
            return new ThinUser(user.Id, user.PhoneNumber, user.Name, user.DateOfBirth, user.Reputation, numFollowers);
        }

        public ThinEvent FindEvent(Guid id)
        {
            Event @event;
            ThinnerUser Host;
            using (_context = new QueryContext())
            {
                @event = _context.Events.Find(id);
                Host = _context.EventLinks.Where(l => l.EventId == id && l.Type == EventLink.EventLinkType.Hosting).Include(l => l.Self).Select(l => new ThinnerUser(l.Self.Id, l.Self.Name)).Single();
            }
            return new ThinEvent(@event.Id,  Host, @event.Name, "Null", @event.StartTime, @event.Latitude, @event.Longitude);
        }

        public List<ThinnerEvent> FindEvents(float latitude, float longitude, float distance)
        {
            throw new NotImplementedException();
        }

        public bool CreateEvent(Guid hostId, string name, string eventType, DateTime startTime, float latitude, float longitude)
        {
            throw new NotImplementedException();
        }

        public bool AddUserToEvent(Guid userId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveUserFromEvent(Guid userId, Guid eventId)
        {
            throw new NotImplementedException();
        }

        public bool EndEvent(Guid Id)
        {
            throw new NotImplementedException();
        }

        public List<ThinnerUser> GetGuestList(Guid Id)
        {
            throw new NotImplementedException();
        }

        // Event Queries


    }
}
