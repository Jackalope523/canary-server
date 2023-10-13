using Server.Boundaries;
using Server.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;

namespace Server.Controls
{
    internal class AccountManager : AbstractManager, IAccountOperations
    {
        public AccountManager(CoreTerminal terminal) : base(terminal) { }

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
            Accounts.UpdateUser(editUser.Id, edits);
		}

        public async Task DeleteUserAsync(Guid userID)
        {
            bool success = Accounts.DeleteUser(userID);
            if (!success)
            { throw new UnexpectedFailureException("User deletion failed."); }
        }

        public async Task UpdateUserLocationAsync(Guid userID, double latitude, double longitude)
		{
			var user = await GetUser(userID);
            await user.SyncLocation();

            user.LastKnownLocation = new() { Latitude = latitude, Longitude = longitude };

            await user.HandleHaunt();

            Accounts.UpdateRecentLocation(user.Id, user.LastKnownLocation.Latitude, user.LastKnownLocation.Longitude, user.LastKnownRadius.Metres);
            Accounts.UpdateHaunt(user.Id, user.Haunt.Latitude, user.Haunt.Longitude, user.HauntRadius.Metres, user.HauntStability);
        }


        internal async Task<User> GetUser(string phoneNumber)
        {
            User user = new(Accounts.FindUserByPhoneNumber(phoneNumber));

            // Check if user account is locked
            if (user.IsLocked)
            { throw new InvalidUserException("User account is locked."); }

            return user;
        }

        internal async Task<(double Latitude, double Longitude, double Radius, int Stability)>
            GetUserHauntAsync(Guid userID)
        {
            return Accounts.GetUserHaunt(userID);
        }

        internal async Task<(double Latitude, double Longitude, double Radius)>
            GetLastKnownUserLocationAsync(Guid userID)
        {
            return Accounts.GetRecentUserLocation(userID);
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
                Accounts.FindUserByEmail(normalisedEmail);
				emailTaken = true;
			}
			catch { }

			if (emailTaken)
			{ throw new InvalidUserException("Email already registered."); }
        }
    }
}
