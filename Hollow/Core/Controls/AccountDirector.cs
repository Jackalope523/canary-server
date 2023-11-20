using Core.Boundaries;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;

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
            if (!ContentValidation.TryNormalisePhoneNumber(phoneNumber, out string normalisedPhoneNumber))
            { throw new ArgumentException($"{nameof(phoneNumber)} must be a valid phone number."); }
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
            bool success = Accounts.CreateUser(newUser.PhoneNumber, email, newUser.Email,
                newUser.Name, newUser.DateOfBirth, CharacterVector.Default.ToCharacter());
            if (!success)
            { throw new UnexpectedFailureException("User creation failed."); }
        }

        public async Task EditUserAsync(ulong userId,
            string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null)
        {
            // Throws if user not found or locked
            User editUser = await GetUser(userId);
            
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
                edits.Add((nameof(UserShard.PhoneNumber), editUser.PhoneNumber));
			}
			if (emailChanged)
			{
                await ThrowIfEmailTaken(editUser.Email);
                edits.Add((nameof(UserShard.Email), email));
                edits.Add(("NormalisedEmail", editUser.Email));
			}
			if (!string.IsNullOrEmpty(name))
			{
                edits.Add((nameof(UserShard.Name), editUser.Name));
			}
            // Internal attributes
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
            Accounts.UpdateUser(editUser.Id, edits);
		}

        public async Task DeleteUserAsync(ulong userId)
        {
            bool success = Accounts.DeleteUser(userId);
            if (!success)
            { throw new UnexpectedFailureException("User deletion failed."); }
        }

        public async Task UpdateUserLocationAsync(ulong userId, double latitude, double longitude)
		{
			var user = await GetUser(userId);
            await user.SyncLocation();

            user.LastKnownLocation = new() { Latitude = latitude, Longitude = longitude };

            await user.HandleHaunt();

            Accounts.UpdateRecentLocation(user.Id, user.LastKnownLocation.Latitude, user.LastKnownLocation.Longitude, user.LastKnownRadius.Metres);
            Accounts.UpdateHaunt(user.Id, user.Haunt.Latitude, user.Haunt.Longitude, user.HauntRadius.Metres, user.HauntStability);

            Event currentEvent = new(Events.FindCurrentEventForUser(user.Id));
			List<EventShard> upcomingEvents;

            // Check if we are at an event
            if (currentEvent != null)
            {
                // Check if we left the event radius
                if (!GeoLocation.AreInRange(user.LastKnownLocation, currentEvent.Location, currentEvent.Radius))
                {
                    // Check if we are a guest or the host
                    if (currentEvent.Host.Id.Equals(user.Id))
                    {
                        // End the event if we are the host
                        await Terminal.EventDirector.EndEventAsync(user.Id, currentEvent.Id);
                    }
                    else
                    {
                        // Leave the event if we are a guest
                        await Terminal.EventDirector.LeaveEventAsync(user.Id, currentEvent.Id);
                    }
                }
                // Check if we are the host
                else if (currentEvent.Host.Id.Equals(user.Id))
                {
                    // Update the position of the event
                    Events.UpdateEvent(currentEvent.Id, new() { (nameof(EventShard.Latitude), user.LastKnownLocation.Latitude),
                        (nameof(EventShard.Longitude), user.LastKnownLocation.Longitude) });
                }
            }
            // Check if on our way to an event
            else if (currentEvent == null &&
                (upcomingEvents = Events.FindUpcomingEventsForUser(user.Id)).Count > 0)
            {
                Event nextEvent = new(upcomingEvents[0]);

                // Check if user is close enough to be a guest
                if (nextEvent.IsInRange(user))
                {
                    Events.SetUserState(user.Id, nextEvent.Id, EventUserState.Present);
                }
            }
        }

		#endregion

		#region Favours

		internal async Task<User> GetUser(string phoneNumber)
        {
            User user = new(Accounts.FindUserByPhoneNumber(phoneNumber));

            // Check if user account is locked
            if (user.IsLocked)
            { throw new InvalidUserException("User account is locked."); }

            return user;
        }

        internal async Task<(double Latitude, double Longitude, double Radius, int Stability)>
            GetUserHauntAsync(ulong userId)
        {
            return Accounts.GetUserHaunt(userId);
        }

        internal async Task<(double Latitude, double Longitude, double Radius)>
            GetLastKnownUserLocationAsync(ulong userId)
        {
            return Accounts.GetRecentUserLocation(userId);
        }

		#endregion

		#region Tools

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
                Accounts.FindUserByEmail(normalisedEmail);
				emailTaken = true;
			}
			catch { }

			if (emailTaken)
			{ throw new InvalidUserException("Email already registered."); }
        }

		#endregion
	}
}
