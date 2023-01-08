using Server.Boundaries;
using DataAccess.Entities;
using Microsoft.Identity.Client;

namespace DataAccess
{
    internal class QueryStore : IAccountDatabase
    {
        public void BlockUser(Guid selfId, Guid targetId)
        {
            throw new NotImplementedException();
        }

        public bool CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(Guid accountID)
        {
            throw new NotImplementedException();
        }

        public ThinUser FindUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public ThinUser FindUser(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public void FollowUser(Guid selfId, Guid targetId)
        {
            throw new NotImplementedException();
        }

        public List<ThinnerUser> GetBlockedUsers(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<ThinnerUser> GetFollowedUsers(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UnblockUser(Guid selfId, Guid targetId)
        {
            throw new NotImplementedException();
        }

        public void UnfollowUser(Guid selfId, Guid targetId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateName(Guid id, string newName)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePasskey(Guid id, string Passkey)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePhoneNumber(Guid id, string newNumber)
        {
            throw new NotImplementedException();
        }

        public bool UpdateReputation(Guid id, int newReputation)
        {
            throw new NotImplementedException();
        }
    }
}
