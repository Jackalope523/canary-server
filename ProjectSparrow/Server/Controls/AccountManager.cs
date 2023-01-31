using Server.Boundaries;
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

        public async Task<ThinUser> GetUserAsync(Guid userID)
        {
            ThinUser targetUser = accounts.FindUser(userID);

            if (targetUser == null)
            { throw new InvalidUserException("Target user not found."); }

            return targetUser;
        }

        public async Task<ThinUser> GetUserAsync(string phoneNumber)
		{
			ThinUser targetUser = accounts.FindUser(phoneNumber);

			if (targetUser == null)
			{ throw new InvalidUserException("Target user not found."); }

			return targetUser;
		}

        public async Task<ThinProfile> GetUserProfileAsync(Guid userID, Guid targetID)
        {
            throw new NotImplementedException();
        }

        public async Task CreateUserAsync(string phoneNumber, string email, string name, DateTime dateOfBirth)
        {
            // Check phone number not in use

            if (accounts.FindUser(phoneNumber) != null)
            { throw new InvalidUserException("Phone Number already registered."); }

            // Create profile

            User newUser = new(phoneNumber, name)
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
            
            bool success = accounts.CreateUser(newUser.Identification, email,
                newUser.Name, newUser.DateOfBirth);

            if (!success)
            { throw new UnexpectedFailureException("User creation failed."); }
        }

        public async Task EditUserAsync(Guid userID,
            string phoneNumber = "", string email = "", string name = "",
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = "", DateTimeOffset? lockoutDate = null, int? accessTries = null)
        {            
            // TODO Verify updates are valid

            ThinUser account = accounts.FindUser(userID);

			if (account == null)
			{ throw new InvalidUserException("Target user not found."); }

			if (phoneNumber != "")
            {
                accounts.UpdatePhoneNumber(userID, phoneNumber);
			}
			if (email != "")
			{
                // NO-OP
			}
			if (name != "")
			{
                accounts.UpdateName(userID, name);
			}
			if (isPhoneNumberConfirmed.HasValue)
			{
				// NO-OP
			}
			if (isEmailConfirmed.HasValue)
			{
				// NO-OP
			}
			if (securityStamp != "")
			{
				// NO-OP
			}
			if (lockoutDate.HasValue)
			{
				// NO-OP
			}
			if (accessTries.HasValue)
			{
				// NO-OP
			}
		}

        public async Task DeleteUserAsync(Guid userID)
        {
            bool success = accounts.DeleteUser(userID);

            if (!success)
            { throw new UnexpectedFailureException("User deletion failed."); }
        }

        public async Task<List<ThinnerUser>> GetFollowedUsersAsync(Guid userID)
        {
            return accounts.GetFollowedUsers(userID);
		}

        public async Task<List<ThinnerUser>> GetBlockedUsersAsync(Guid userID)
		{
			return accounts.GetBlockedUsers(userID);
		}

        public async Task FollowUserAsync(Guid userID, Guid targetID)
        {
            // TODO Check target account exists

            accounts.FollowUser(userID, targetID);
		}

		public async Task UnfollowUserAsync(Guid userID, Guid targetID)
        {
            // TODO Check target account exists

            accounts.UnfollowUser(userID, targetID);
        }

		public async Task BlockUserAsync(Guid userID, Guid targetID)
        {
            // TODO Check target account exists

            accounts.BlockUser(userID, targetID);
        }

		public async Task UnblockUserAsync(Guid userID, Guid targetID)
        {
            // TODO Check target account exists

            accounts.UnblockUser(userID, targetID);
        }
	}
}
