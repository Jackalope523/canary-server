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
using Microsoft.Extensions.Logging;

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
            var targetUser = accounts.FindUser(userID);
            if (targetUser == null)
            { throw new InvalidUserException("User not found."); }

            return targetUser;
        }

        public async Task<ThinUser> GetUserAsync(string phoneNumber)
		{
			var targetUser = accounts.FindUser(phoneNumber);
			if (targetUser == null)
			{ throw new InvalidUserException("User not found."); }

			return targetUser;
		}

        public async Task<ThinProfile> GetUserProfileAsync(Guid userID, Guid targetID)
        {
            // Check that user is not blocked
            if (await UserIsBlocked(userID, targetID))
            {
                throw new InvalidUserException("User is unable to view target.");
            }

            var targetUser = accounts.FindUser(targetID);
            return new ThinProfile(targetID, targetUser.Name, targetUser.Reputation, targetUser.NumberOfFollowers);
        }

        public async Task CreateUserAsync(string phoneNumber, string email, string name, DateTime dateOfBirth)
        {
            // Check phone number not in use
            if (accounts.FindUser(phoneNumber) != null)
            { throw new InvalidUserException("Phone Number already registered."); }

            // TODO Normalise data
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
			if (accounts.FindUser(userID) == null)
			{ throw new InvalidUserException("User not found."); }

            // TODO Verify updates are valid
            // TODO Normalise data
            // Update individual attributes
			if (phoneNumber != "")
            {
                accounts.UpdatePhoneNumber(userID, phoneNumber);
			}
			if (email != "")
			{
                accounts.UpdateEmail(userID, email);
			}
			if (name != "")
			{
                accounts.UpdateName(userID, name);
			}
			if (isPhoneNumberConfirmed.HasValue)
			{
				accounts.UpdatePhoneConfirmation(userID, isPhoneNumberConfirmed.Value);
			}
			if (isEmailConfirmed.HasValue)
			{
				accounts.UpdateEmailConfirmation(userID, isEmailConfirmed.Value);
			}
			if (securityStamp != "")
			{
				accounts.UpdateSecurityStamp(userID, securityStamp);
			}
			if (lockoutDate.HasValue)
			{
				accounts.UpdateLockoutDate(userID, lockoutDate.Value);
			}
			if (accessTries.HasValue)
			{
				accounts.UpdateAccessTries(userID, accessTries.Value);
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
            accounts.FollowUser(userID, targetID);
		}

		public async Task UnfollowUserAsync(Guid userID, Guid targetID)
        {
            accounts.UnfollowUser(userID, targetID);
        }

		public async Task BlockUserAsync(Guid userID, Guid targetID)
        {
            accounts.BlockUser(userID, targetID);
        }

		public async Task UnblockUserAsync(Guid userID, Guid targetID)
        {
            accounts.UnblockUser(userID, targetID);
		}

		private async Task<bool> UserIsBlocked(Guid userID, Guid targetID)
        {
			var targetBlockedList = accounts.GetBlockedUsers(targetID);

			// Check if user is blocked by target
			if (targetBlockedList.Find(x => x.Id == userID) != null)
			{
				return false;
			}

			return true;
		}
	}
}
