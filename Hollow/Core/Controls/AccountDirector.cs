using Core.Boundaries;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class AccountDirector : AbstractDirector, IAccountOperations
    {
		#region Initialisation

		public AccountDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<UserShard> GetUserAsync(ulong userId)
        {
            return (await GetUser(userId)).ToUserShard();
        }

        public async Task<UserShard> GetUserAsync(string phoneNumber)
		{
            Try(ContentValidation.TryNormalisePhoneNumber(phoneNumber, out string normalisedPhoneNumber),
                new ArgumentException($"{nameof(phoneNumber)} must be a valid phone number."));

            return (await GetUser(normalisedPhoneNumber)).ToUserShard();
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
                JoinDate = Time
            };

            // Validate and normalise user
            Try(newUser.ValidateAndNormalise(),
                new InvalidInformationException("Invalid account details provided."));

            // Check if phone number is in use
            await ThrowIfPhoneNumberTaken(newUser.PhoneNumber);

            // Check if email is in use
            if (!string.IsNullOrEmpty(email))
            { await ThrowIfEmailTaken(newUser.Email); }

            // Store profile
            Try(Accounts.CreateUser(newUser.PhoneNumber, email, newUser.Email,
                newUser.Name, newUser.DateOfBirth, CharacterVector.Default.ToCharacter()),
                new UnexpectedFailureException("User creation failed."));
        }

        public async Task EditUserAsync(ulong userId,
            string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null)
        {
            // Throws if user not found or locked
            var user = await GetUser(userId);
            
            // Check unique details changed to avoid errors
            bool phoneNumberChanged = !string.IsNullOrEmpty(phoneNumber) && user.PhoneNumber != phoneNumber;
            bool emailChanged = !string.IsNullOrEmpty(email) && user.Email != email;

            // Modify user for validation
            user.PhoneNumber = string.IsNullOrEmpty(phoneNumber) ? user.PhoneNumber : phoneNumber;
            user.Email = string.IsNullOrEmpty(email) ? user.Email : email;
            user.Name = string.IsNullOrEmpty(name) ? user.Name : name;

            // Validate and Normalise
            Try(user.ValidateAndNormalise(),
                new InvalidInformationException("Invalid details provided."));

            List<(string Property, object Value)> edits = new();

            // Gather individual edits
			if (phoneNumberChanged)
            {
                await ThrowIfPhoneNumberTaken(user.PhoneNumber);
                edits.Add((nameof(UserShard.PhoneNumber), user.PhoneNumber));
			}
			if (emailChanged)
			{
                await ThrowIfEmailTaken(user.Email);
                edits.Add((nameof(UserShard.Email), email));
                edits.Add(("NormalisedEmail", user.Email));
			}
			if (!string.IsNullOrEmpty(name))
			{
                edits.Add((nameof(UserShard.Name), user.Name));
			}
            // Internal attributes for account store
			if (isPhoneNumberConfirmed.HasValue)
			{
                edits.Add((nameof(UserShard.IsPhoneConfirmed), isPhoneNumberConfirmed.Value));
			}
			if (isEmailConfirmed.HasValue)
			{
                edits.Add((nameof(UserShard.IsEmailConfirmed), isEmailConfirmed.Value));
			}
			if (!string.IsNullOrEmpty(securityStamp))
			{
                edits.Add((nameof(UserShard.SecurityStamp), securityStamp));
			}
			if (lockoutDate.HasValue)
			{
                edits.Add((nameof(UserShard.LockoutDate), lockoutDate.Value));
			}
			if (accessTries.HasValue)
			{
                edits.Add((nameof(UserShard.AccessTries), accessTries.Value));
			}

            // Push update
            Accounts.UpdateUser(user.Id, edits);
		}

        public async Task DeleteUserAsync(ulong userId)
        {
            Try(Accounts.DeleteUser(userId),
                new UnexpectedFailureException("User deletion failed."));
        }

        public async Task UpdateUserLocationAsync(ulong userId, double latitude, double longitude)
		{
			var user = await GetUser(userId);
            await user.SyncLocation();
            var userIsAtEvent = user.IsAtEvent();
            var upcomingEventsSync = user.SyncUpcomingEvents();

			user.LastKnownLocation = new() { Latitude = latitude, Longitude = longitude };
            await user.HandleHaunt();

            // Position updates
            Accounts.UpdateRecentLocation(user.Id, user.LastKnownLocation.Latitude, user.LastKnownLocation.Longitude, user.LastKnownRadius.Metres);
            Accounts.UpdateHaunt(user.Id, user.Haunt.Latitude, user.Haunt.Longitude, user.HauntRadius.Metres, user.HauntStability);

            await upcomingEventsSync;
            var nextEvent = await user.NextEvent();

            // Check if user is at an event
            if (await userIsAtEvent)
            {
                // Check if user left the event radius
                if (!GeoLocation.AreInRange(user.LastKnownLocation, user.CurrentEvent.Location, user.CurrentEvent.Radius))
                {
                    // Check if user is a guest or the host
                    if (user.CurrentEvent.IsHostedBy(user))
                    {
                        // End the event if user is the host
                        await Terminal.EventDirector.EndEventAsync(user.Id, user.CurrentEvent.Id);
                    }
                    else
                    {
                        // Leave the event if user is a guest
                        await Terminal.EventDirector.LeaveEventAsync(user.Id, user.CurrentEvent.Id);
                    }
                }
                // Check if user is the host
                else if (user.CurrentEvent.IsHostedBy(user))
                {
                    // Update the position of the event
                    Events.UpdateEvent(user.CurrentEvent.Id, new() { (nameof(EventShard.Latitude), user.LastKnownLocation.Latitude),
                        (nameof(EventShard.Longitude), user.LastKnownLocation.Longitude) });
                }
            }
            // Check if user is on their way to an event
            else if (!await userIsAtEvent &&
                nextEvent != null)
            {
                // Check if user is close enough to be a guest
                if (nextEvent.IsInRange(user))
                {
                    Events.SetUserState(user.Id, nextEvent.Id, EventUserState.Present);
                }
            }
        }

		#endregion

		#region Favours

        internal async Task UpdateAllAsync(List<User> users, Func<User,List<(string Property, object Value)>> edits)
        {
            users.ForEach(user => Accounts.UpdateUser(user.Id, edits(user)));
		}

        internal async Task<(double Latitude, double Longitude, double Radius, int Stability)>
            RequestUserHauntAsync(User user)
        {
            return Accounts.GetUserHaunt(user.Id);
        }

        internal async Task<(double Latitude, double Longitude, double Radius)>
            RequestLastKnownUserLocationAsync(User user)
        {
            return Accounts.GetRecentUserLocation(user.Id);
        }

		#endregion

		#region Tools

		private async Task<User> GetUser(string phoneNumber)
        {
            User user = new(Accounts.FindUserByPhoneNumber(phoneNumber));

            // Check if user account is locked
            Fail(user.IsLocked,
                new InvalidUserException("User account is locked."));

            return user;
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

            Fail(numberTaken,
                new InvalidUserException("Phone Number already registered."));
		}

        private async Task ThrowIfEmailTaken(string normalisedEmail)
        {
			bool emailTaken = false;
			try
			{
                // Throws an exception if there is no user
                Accounts.FindUserByEmail(normalisedEmail);
				emailTaken = true;
			}
			catch { }

			Fail(emailTaken,
                new InvalidUserException("Email already registered."));
        }

		#endregion
	}
}
