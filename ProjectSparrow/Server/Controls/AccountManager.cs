using Server.Boundaries.;
using Server.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Shared;

namespace Server.Controls
{
    internal class AccountManager : IAccountOperations
    {
        private IAccountDatabase accounts { get; init; }

        public AccountManager(IAccountDatabase accountDatabase)
        {
            accounts = accountDatabase;
        }

        public ThinProfile GetUserProfile(Guid userID, Guid targetID)
        {
            // TODO Verify user

            ThinUser targetUser = accounts.FindUser(targetID);

            if (targetUser == null)
            { throw new InvalidUserException("Target user not found."); }

            return new ThinProfile(targetUser.AccountId, targetUser.Name,
                targetUser.Reputation, targetUser.NumberOfFollowers);
        }

        public string TryLogin(string identification, string passkey)
        {
            // TODO Add security manager
            throw new NotImplementedException();
        }

        public void CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth)
        {
            // Check phone number not in use

            if (accounts.FindUser(phoneNumber) != null)
            { throw new InvalidUserException("Phone Number already registered."); }

            // Create profile

            User newUser = new(phoneNumber, name, passkey)
            {
                DateOfBirth = dateOfBirth,
                JoinDate = DateTime.Now,
                Verified = false
            };

            // Validate profile

            bool valid = newUser.ValidateUser();

            if (!valid)
            { throw new InvalidInformationException("Invalid account details provided."); }

            // Store profile
            
            bool success = accounts.CreateUser(newUser.Identification, passkey,
                newUser.Name, newUser.DateOfBirth);

            if (!success)
            { throw new UnexpectedFailureException("User creation failed."); }
        }

        public void EditUser(Guid userID, string newName)
        {
            // TODO Verify user
            
            // Verify updates are valid

            ThinUser account = accounts.FindUser(userID);

            // TODO Use only what is given

			User userValidation = new("", newName, "")
			{
                AccountID = userID
			};

			// Validate profile

			bool valid = userValidation.ValidateUser();

			if (!valid)
            { throw new InvalidInformationException("Invalid account details provided."); }

            bool success = accounts.UpdateName(userID, newName);

            if (!success)
            { throw new UnexpectedFailureException("User update failed."); }
        }

        public void DeleteUser(Guid userID)
        {
            // TODO Verify user

            bool success = accounts.DeleteUser(userID);

            if (!success)
            { throw new UnexpectedFailureException("User deletion failed."); }
        }

        public List<ThinnerUser> GetFollowedUsers(Guid userID)
        {
            // TODO Verify user

            return accounts.GetFollowedUsers(userID);
		}

        public List<ThinnerUser> GetBlockedUsers(Guid userID)
		{
            // TODO Verify user
            
			return accounts.GetBlockedUsers(userID);
		}

        public void FollowUser(Guid userID, Guid targetID)
        {
            // TODO Verify user

            // TODO Check target account exists

            accounts.FollowUser(userID, targetID);
		}

		public void UnfollowUser(Guid userID, Guid targetID)
        {
            // TODO Verify user

            // TODO Check target account exists

            accounts.UnfollowUser(userID, targetID);
        }

		public void BlockUser(Guid userID, Guid targetID)
        {
            // TODO Verify user

            // TODO Check target account exists

            accounts.BlockUser(userID, targetID);
        }

		public void UnblockUser(Guid userID, Guid targetID)
        {
            // TODO Verify user

            // TODO Check target account exists

            accounts.UnblockUser(userID, targetID);
        }
	}
}
