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
using PhoneNumbers;

namespace Server.Controls
{
    internal class AccountManager : IAccountOperations
    {
        internal static AccountManager Manager { get; private set; }

        private IAccountDatabase accounts { get; init; }
        private IEventDatabase events { get; init; }

        public AccountManager(IAccountDatabase accountDatabase, IEventDatabase eventDatabase)
        {
            Manager = this;

            accounts = accountDatabase;
            events = eventDatabase;
        }

        public async Task<ThinUser> GetUserAsync(Guid userID)
        {
            return (await GetUser(userID)).ToThinUser();
        }

        public async Task<ThinUser> GetUserAsync(string phoneNumber)
		{
            return (await GetUser(phoneNumber)).ToThinUser();
		}

        public async Task<ThinProfile> GetUserProfileAsync(Guid userID, Guid targetID)
        {
            var user = await GetUser(userID);
            var targetUser = await GetUser(targetID);

            // Check if user is blocked
            if (await targetUser.IsBlocking(user))
            { throw new InvalidUserException("User is unable to view target."); }

            return targetUser.ToThinProfile();
        }

        public async Task<List<ThinEvent>> GetUserActivityAsync(Guid userID, Guid targetID)
        {
            var user = await GetUser(userID);
            var targetUser = await GetUser(targetID);

            // Check if users are friends
            if (!await targetUser.IsFriendsWith(user)) 
            { throw new InvalidUserException("User is unable to view target."); }

            // Gather active and upcoming events
            var upcomingActivity = await GetUserActivityInternalAsync(targetID);

            // Remove active and upcoming events if the user cannot view them
            await EventManager.Manager.RemoveInaccessibleEventsAsync(user, upcomingActivity);

            return upcomingActivity.ToList();
        }

        public async Task<Dictionary<ThinnerUser, List<ThinEvent>>> GetFriendActivityAsync(Guid userID)
        {
            var user = await GetUser(userID);
            var friends = accounts.GetFriends(userID);

            Dictionary<ThinnerUser, List<ThinEvent>> friendEvents = new();

            // Gather visible activity of each friend
			foreach (var friend in friends)
            {
                var friendActivity = await GetUserActivityInternalAsync(friend.Id);
                await EventManager.Manager.RemoveInaccessibleEventsAsync(user, friendActivity);
                friendEvents.Add(friend, friendActivity);
            }

            return friendEvents;
        }

        public async Task CreateUserAsync(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth)
        {
            // Create user
            User newUser = new()
            {
                PhoneNumber = phoneNumber,
                Email = email,
                Name = name,
                DateOfBirth = dateOfBirth,
                JoinDate = DateTimeOffset.UtcNow
            };

            // Validate and normalise user
            bool valid = newUser.ValidateAndNormalise();
            if (!valid)
            { throw new InvalidInformationException("Invalid account details provided."); }

            // Check if phone number is in use
            await ThrowIfPhoneNumberTaken(newUser.PhoneNumber);

            // Check if email is in use
            if (!string.IsNullOrEmpty(email))
            { await ThrowIfEmailTaken(newUser.Email); }

            // Store profile
            bool success = accounts.CreateUser(newUser.PhoneNumber, email,
                newUser.Name, newUser.DateOfBirth);
            if (!success)
            { throw new UnexpectedFailureException("User creation failed."); }

            accounts.UpdateNormalisedEmail(newUser.Id, newUser.Email);
        }

        public async Task EditUserAsync(Guid userID,
            string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null)
        {
            // Throws if user not found or locked
            User editUser = await GetUser(userID);
            
            editUser.PhoneNumber = phoneNumber;
            editUser.Email = email;
            editUser.Name = name;

            // Validate and Normalise
            if (!editUser.ValidateAndNormalise())
            { throw new InvalidInformationException("Invalid details provided."); }

            // Update individual attributes
			if (!string.IsNullOrEmpty(phoneNumber))
            {
                await ThrowIfPhoneNumberTaken(editUser.PhoneNumber);
                accounts.UpdatePhoneNumber(userID, editUser.PhoneNumber);
			}
			if (!string.IsNullOrEmpty(email))
			{
                await ThrowIfEmailTaken(editUser.Email);
                accounts.UpdateEmail(userID, email);
                accounts.UpdateNormalisedEmail(userID, editUser.Email);
			}
			if (!string.IsNullOrEmpty(name))
			{
                accounts.UpdateName(userID, editUser.Name);
			}

            // Internal attributes
			if (isPhoneNumberConfirmed.HasValue)
			{
				accounts.UpdatePhoneConfirmation(userID, isPhoneNumberConfirmed.Value);
			}
			if (isEmailConfirmed.HasValue)
			{
				accounts.UpdateEmailConfirmation(userID, isEmailConfirmed.Value);
			}
			if (!string.IsNullOrEmpty(securityStamp))
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

        public async Task RateUser(Guid userID, Guid targetID, UserRating rating)
        {
            await GetUser(userID);
            accounts.RateUser(userID, targetID, rating);
        }

        public async Task ReportUserAsync(Guid userID, Guid targetID, UserReportType reportType, string reportDetails)
        {
            accounts.ReportUser(userID, targetID, reportType, reportDetails);

			// Compute user's standing
			var user = await GetUser(targetID);
			var status = await user.Reported();

			// Check if host should be punished
			if (user.AccountStatus != status)
			{
				accounts.UpdateAccountStatus(user.Id, status);
			}
		}



        internal async Task<User> GetUser(Guid userID)
        {
			User user = new(accounts.FindUser(userID));

			// Check if user account is locked
			if (user.IsLocked)
			{ throw new InvalidUserException("User account is locked."); }

			return user;
		}

        internal async Task<User> GetUser(string phoneNumber)
        {
            User user = new(accounts.FindUser(phoneNumber));

            // Check if user account is locked
            if (user.IsLocked)
            { throw new InvalidUserException("User account is locked."); }

            return user;
        }

        internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)> GetAllReportsAsync(Guid userID)
        {
            return accounts.GetReports(userID);
        }



        private async Task ThrowIfPhoneNumberTaken(string phoneNumber)
        {
			bool numberTaken = false;
			try
			{
				// Throws an exception if there is no user
				await GetUser(phoneNumber);
				numberTaken = true;
			}
			catch { }

			if (numberTaken)
			{ throw new InvalidUserException("Phone Number already registered."); }
		}

        private async Task ThrowIfEmailTaken(string normalisedEmail)
        {
			bool emailTaken = false;
			try
			{
                // Throws an exception if there is no user
                accounts.FindUserByEmail(normalisedEmail);
				emailTaken = true;
			}
			catch { }

			if (emailTaken)
			{ throw new InvalidUserException("Email already registered."); }
        }

        private async Task<List<ThinEvent>> GetUserActivityInternalAsync(Guid userID)
        {
            // Gather all user event data
            var upcomingActivity = events.FindUpcomingEvents(userID);
            upcomingActivity.Add(events.FindAttendingEvent(userID));

            return upcomingActivity.ToList();
        }
	}
}
