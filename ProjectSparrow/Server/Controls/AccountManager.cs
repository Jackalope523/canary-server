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

        public async Task<UserShard> GetUserAsync(Guid userID)
        {
            return (await GetUser(userID)).ToThinUser();
        }

        public async Task<UserShard> GetUserAsync(string phoneNumber)
		{
            if (!ContentValidation.TryNormalisePhoneNumber(phoneNumber, out string normalisedPhoneNumber))
            { throw new ArgumentException($"{nameof(phoneNumber)} must be a valid phone number."); }
            return (await GetUser(normalisedPhoneNumber)).ToThinUser();
		}

        public async Task<UserProfile> GetUserProfileAsync(Guid userID, Guid targetID)
        {
            var user = await GetUser(userID);
            var targetUser = await GetUser(targetID);

            // Check if user is blocked
            if (await targetUser.IsBlocking(user))
            { throw new InvalidUserException("User is unable to view target."); }

            return targetUser.ToThinProfile();
        }

        public async Task<List<EventShard>> GetUserActivityAsync(Guid userID, Guid targetID)
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

        public async Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(Guid userID)
        {
            var user = await GetUser(userID);
            var friends = accounts.GetFriends(userID);

            Dictionary<UserSilhouette, List<EventShard>> friendEvents = new();

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
            bool success = accounts.CreateUser(newUser.PhoneNumber, email, newUser.Email,
                newUser.Name, newUser.DateOfBirth, CharacterVector.Default.ToCharacter());
            if (!success)
            { throw new UnexpectedFailureException("User creation failed."); }
        }

        public async Task EditUserAsync(Guid userID,
            string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null)
        {
            // Throws if user not found or locked
            User editUser = await GetUser(userID);
            
            // Check unique details changed to avoid errors
            bool phoneNumberChanged = !string.IsNullOrEmpty(phoneNumber) && editUser.PhoneNumber != phoneNumber;
            bool emailChanged = !string.IsNullOrEmpty(email) && editUser.Email != email;

            // Modify user for validation
            editUser.PhoneNumber = string.IsNullOrEmpty(phoneNumber) ? editUser.PhoneNumber : phoneNumber;
            editUser.Email = string.IsNullOrEmpty(email) ? editUser.Email : email;
            editUser.Name = string.IsNullOrEmpty(name) ? editUser.Name : name;

            // Validate and Normalise
            if ((phoneNumberChanged || emailChanged) && !editUser.ValidateAndNormalise())
            { throw new InvalidInformationException("Invalid details provided."); }

            List<(string Property, object Value)> edits = new();

            // Track individual edits
			if (phoneNumberChanged)
            {
                await ThrowIfPhoneNumberTaken(editUser.PhoneNumber);
                edits.Add(("PhoneNumber", editUser.PhoneNumber));
			}
			if (emailChanged)
			{
                await ThrowIfEmailTaken(editUser.Email);
                edits.Add(("Email", email));
                edits.Add(("NormalisedEmail", editUser.Email));
			}
			if (!string.IsNullOrEmpty(name))
			{
                edits.Add(("Name", editUser.Name));
			}
            // Internal attributes
			if (isPhoneNumberConfirmed.HasValue)
			{
                edits.Add(("IsPhoneConfirmed", isPhoneNumberConfirmed.Value));
			}
			if (isEmailConfirmed.HasValue)
			{
                edits.Add(("IsEmailConfirmed", isEmailConfirmed.Value));
			}
			if (!string.IsNullOrEmpty(securityStamp))
			{
                edits.Add(("SecurityStamp", securityStamp));
			}
			if (lockoutDate.HasValue)
			{
                edits.Add(("LockoutDate", lockoutDate.Value));
			}
			if (accessTries.HasValue)
			{
                edits.Add(("AccessTries", accessTries.Value));
			}

            // Push update
            accounts.UpdateUser(editUser.Id, edits);
		}

        public async Task DeleteUserAsync(Guid userID)
        {
            bool success = accounts.DeleteUser(userID);
            if (!success)
            { throw new UnexpectedFailureException("User deletion failed."); }
        }

        public async Task UpdateUserLocationAsync(Guid userID, double latitude, double longitude)
		{
			var user = await GetUser(userID);
            await user.SyncLocation();

            user.LastKnownLocation = new() { Latitude = latitude, Longitude = longitude };

            await user.HandleHaunt();

            accounts.UpdateRecentLocation(user.Id, user.LastKnownLocation.Latitude, user.LastKnownLocation.Longitude, user.LastKnownRadius.Metres);
            accounts.UpdateHaunt(user.Id, user.Haunt.Latitude, user.Haunt.Longitude, user.HauntRadius.Metres, user.HauntStability);
        }

        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid userID)
        {
            return accounts.GetFollowedUsers(userID);
		}

        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid userID)
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

        public async Task RateUserAsync(Guid userID, Guid targetID, UserRating rating)
        {
            if (rating != UserRating.Remove)
            {
                accounts.RateUser(userID, targetID, rating);
            }
            else
            {
                accounts.RemoveUserRating(userID, targetID);
            }

            User targetUser = new(targetID);
            await targetUser.SyncReputation();
            targetUser.CalculateReputation();
            accounts.UpdateUser(targetID, new() { ("Reputation", targetUser.Reputation) });
        }

        public async Task ReportUserAsync(Guid userID, Guid eventId, Guid targetID, UserReportType reportType, string reportDetails)
        {
            accounts.ReportUser(userID, eventId, targetID, reportType, reportDetails);

			// Compute user's standing
			var user = await GetUser(targetID);
			var status = await user.Reported();

			// Check if host should be punished
			if (user.AccountStatus != status)
			{
                accounts.UpdateUser(targetID, new() { ("AccountStatus", status) });
			}
		}



        internal async Task<User> GetUser(Guid userID)
        {
			User user = new(accounts.FindUserById(userID));

			// Check if user account is locked
			if (user.IsLocked)
			{ throw new InvalidUserException("User account is locked."); }

			return user;
		}

        internal async Task<User> GetUser(string phoneNumber)
        {
            User user = new(accounts.FindUserByPhoneNumber(phoneNumber));

            // Check if user account is locked
            if (user.IsLocked)
            { throw new InvalidUserException("User account is locked."); }

            return user;
        }

        internal async Task<(double Latitude, double Longitude, double Radius, int Stability)> GetUserHauntAsync(Guid userID)
        {
            return accounts.GetUserHaunt(userID);
        }

        internal async Task<(double Latitude, double Longitude, double Radius)> GetLastKnownUserLocationAsync(Guid userID)
        {
            return accounts.GetRecentUserLocation(userID);
        }

        internal async Task<(int Positive, int Negative)> GetAllRatingsAsync(Guid userID)
        {
            return accounts.GetUserRatings(userID);
        }

        internal async Task<(List<UserReport> UserReports, List<EventReport> EventReports)> GetAllReportsAsync(Guid userID)
        {
            return accounts.GetReportsAboutUser(userID);
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

        private async Task<List<EventShard>> GetUserActivityInternalAsync(Guid userID)
        {
            // Gather all user event data
            var upcomingActivity = events.FindUpcomingEventsForUser(userID);
            upcomingActivity.Add(events.FindCurrentEventForUser(userID));

            return upcomingActivity.ToList();
        }

        public Task ReportUserAsync(Guid userID, Guid targetID, UserReportType reportType, string reportDetails)
        {
            throw new NotImplementedException();
        }
    }
}
