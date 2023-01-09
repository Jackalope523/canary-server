using Server.Boundaries;
using DataAccess.Entities;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess
{
    internal class EntryNotFoundException : Exception {
        public EntryNotFoundException(string message) : base(message)
        {
        }
    };  


    internal class QueryStore : IAccountDatabase
    {
        private static QueryContext _context = new QueryContext();

        private static bool Operation(Entity target, Func<Entity,EntityEntry> work)
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

            return Operation(toCreate, u => _context.Users.Add((User)u)); 
        }
        public bool DeleteUser(Guid Id) { return Operation(new User { Id = Id }, u => _context.Users.Remove((User)u)); }

        Func<Entity, EntityEntry> updateUser = u => _context.Users.Update((User)u);
        public bool UpdateName(Guid id, string newName) { return Operation(new User { Id = id, Name = newName }, updateUser); }
        public bool UpdatePasskey(Guid id, string newPasskey) { return Operation(new User { Id = id, Passkey = newPasskey }, updateUser); }
        public bool UpdatePhoneNumber(Guid id, string newNumber) { return Operation(new User { Id = id, PhoneNumber = newNumber }, updateUser); }
        public bool UpdateReputation(Guid id, int newReputation) { return Operation(new User { Id = id, Reputation = newReputation }, updateUser); }

        Func<Entity, EntityEntry> addUserLink = l => _context.UserLinks.Add((UserLink)l);
        Func<Entity, EntityEntry> removeUserLink = l => _context.UserLinks.Remove((UserLink)l);
        public bool FollowUser(Guid selfId, Guid targetId) { return Operation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, addUserLink); }      
        public bool UnfollowUser(Guid selfId, Guid targetId) { return Operation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Following }, removeUserLink); } 
        public bool BlockUser(Guid selfId, Guid targetId) { return Operation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, addUserLink); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return Operation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Blocked }, removeUserLink); }     

        
        public ThinUser FindUser(Guid id)
        {
            using (QueryContext context = new QueryContext())
            {
                var user = context.Users.Find(id);
                if (user.Equals(null))
                {
                    throw new EntryNotFoundException("User could not be found. Are you sure it is in the database?");
                }
                else
                {
                    int numFollowers = context.UserLinks.Where(l => l.SelfId == id && l.Type.Equals(UserLink.UserLinkType.Following)).Count();
                    return new ThinUser(user.Id, user.PhoneNumber, user.Name, user.DateOfBirth, user.Reputation, numFollowers);
                }
            }
        }

        public ThinUser FindUser(string phoneNumber)
        {
            using (QueryContext context = new QueryContext())
            {
                var user = context.Users.Where(u => u.PhoneNumber.Equals(phoneNumber)).Single();
                if (user.Equals(null))
                {
                    throw new EntryNotFoundException("User could not be found. Are you sure it is in the database?");
                }
                else
                {
                    int numFollowers = context.UserLinks.Where(l => l.SelfId == id && l.Type.Equals(UserLink.UserLinkType.Following)).Count();
                    return new ThinUser(user.Id, user.PhoneNumber, user.Name, user.DateOfBirth, user.Reputation, numFollowers);
                }
            }
        }

        public List<ThinnerUser> GetBlockedUsers(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<ThinnerUser> GetFollowedUsers(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
