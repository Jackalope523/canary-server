using Core.Boundaries;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using static Core.Entities.Arbiter;
using static Core.Entities.Artificer;
using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class AccountDirector : AbstractDirector, IAccountOperations
    {
		#region Initialisation

		public AccountDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<CoreUser> GetCoreUserAsync(ulong userId)
        {
            return (await GetUserAsync(userId)).ToCoreUser();
        }

        public async Task<CoreUser> GetCoreUserAsync(string phoneNumber)
		{
            // Verify phone number is valid
            Verify(ContentValidation.TryNormalisePhoneNumber(phoneNumber, out string normalisedPhoneNumber),
                new InvalidInformationException($"{nameof(phoneNumber)} must be a valid phone number."));

            return (await GetUser(normalisedPhoneNumber)).ToCoreUser();
		}

		public async Task<AccountShard> GetAccountShardAsync(ulong userId)
        {
            return (await GetUserAsync(userId)).ToAccountShard();
        }

		public async Task<UserShard> GetUserShardAsync(ulong userId)
        {
            return (await GetUserAsync(userId)).ToUserShard();
        }

        public async Task CreateUserAsync(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth, string code = "")
        {
            CoreBanner banner;

            // Verify banner code
            try
            {
                banner = await Banners.FindBannerByCodeAsync(code.ToLower());
            }
            catch
            { throw new InvalidInformationException("Incorrect code."); }

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
            Verify(newUser.ValidateAndNormalise(out string issues),
                new InvalidInformationException($"Invalid account details provided. Issues: {issues}"));

            // Verify phone number is not in use
            await ThrowIfPhoneNumberTaken(newUser.PhoneNumber);

            // Check if email is in use
            if (!string.IsNullOrEmpty(email))
            { await ThrowIfEmailTaken(newUser.Email); }

            // Store nest
            var user = await Accounts.CreateUserAsync(newUser.PhoneNumber, email, newUser.Email,
                newUser.Name, newUser.DateOfBirth, Time, CharacterVector.Default(newUser.GetAge()).ToCharacter());

            // Add user to banner
            await Banners.AddUserToBannerAsync(user.Id, banner.Id, Time);
        }

        public async Task EditUserAsync(ulong userId,
            string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null)
        {
            // Throws if user not found or locked
            var user = await base.GetUserAsync(userId);
            
            // Check unique details changed to avoid errors
            bool phoneNumberChanged = !string.IsNullOrEmpty(phoneNumber) && user.PhoneNumber != phoneNumber;
            bool emailChanged = !string.IsNullOrEmpty(email) && user.Email != email;
            bool nameChanged = !string.IsNullOrEmpty(name);

            // Modify user for validation
            user.PhoneNumber = phoneNumberChanged ? phoneNumber : user.PhoneNumber;
            user.Email = emailChanged ? email : user.Email;
            user.Name = nameChanged ? name : user.Name;

            // Validate and Normalise
            Verify(user.ValidateAndNormalise(out string issues),
                new InvalidInformationException($"Invalid details provided. Issues: {issues}"));

            List<(string Property, object Value)> edits = new();

            // Gather individual edits
			if (phoneNumberChanged)
            {
                await ThrowIfPhoneNumberTaken(user.PhoneNumber);
                edits.Add((nameof(CoreUser.PhoneNumber), user.PhoneNumber));
                // edits.Add((nameof(CoreUser.IsPhoneConfirmed), false));
            }
			if (nameChanged)
			{
                edits.Add((nameof(CoreUser.Name), user.Name));
			}
			if (emailChanged)
			{
                await ThrowIfEmailTaken(user.Email);
                edits.Add((nameof(CoreUser.Email), email));
                edits.Add(("NormalisedEmail", user.Email));
                edits.Add((nameof(CoreUser.IsEmailConfirmed), false));
            }
            // Internal attributes for account store
			if (IsNotNull(isPhoneNumberConfirmed))
			{
                edits.Add((nameof(CoreUser.IsPhoneConfirmed), isPhoneNumberConfirmed.Value));
			}
			if (IsNotNull(isEmailConfirmed))
			{
                edits.Add((nameof(CoreUser.IsEmailConfirmed), isEmailConfirmed.Value));
			}
			if (!string.IsNullOrEmpty(securityStamp))
			{
                edits.Add((nameof(CoreUser.SecurityStamp), securityStamp));
			}
			if (IsNotNull(lockoutDate))
			{
                edits.Add((nameof(CoreUser.LockoutDate), lockoutDate.Value));
			}
			if (IsNotNull(accessTries))
			{
                edits.Add((nameof(CoreUser.AccessTries), accessTries.Value));
			}

            // Push update
            await Accounts.UpdateUserAsync(user.Id, edits);
		}

        public async Task UpdateUserAgreement(ulong userId)
        {
            var user = await GetUserAsync(userId);

            await Accounts.UpdateUserAsync(user.Id,
                new() { (nameof(CoreUser.TimeOfUserAgreement), Time) });
        }

        public async Task EditAvatarAsync(ulong userId, MemoryStream image)
        {
            var user = await GetUserAsync(userId);

            await Terminal.MediaDirector.UploadAvatarAsync(user.Id, image);
        }

        public async Task DeleteUserAsync(ulong userId)
        {
            await Accounts.DeleteUserAsync(userId);
        }

        public async Task UpdateUserLocationAsync(ulong userId, double latitude, double longitude)
		{
			var user = await base.GetUserAsync(userId);
            var userIsAtGathering = user.IsAtGathering();

            user.LastKnownLocation.Set(new() { Latitude = latitude, Longitude = longitude });
            await user.HandleHaunt();

            Log.LogWarning("Updating location for user {id} {name} to {latitude}, {longitude} at {time}",
                user.Id, user.Name, latitude, longitude, Time);

            // Position update
            _ = Accounts.UpdateRecentLocationAsync(user.Id,
                (await user.LastKnownLocation).Latitude,
                (await user.LastKnownLocation).Longitude,
                (await user.LastKnownRadius).Metres);
            // Haunt update
            _ = Accounts.UpdateHauntAsync(user.Id,
                (await user.Haunt).Latitude,
                (await user.Haunt).Longitude,
                (await user.HauntRadius).Metres,
                await user.HauntStability);

            var nextGathering = await user.NextGathering();

            // Check if user is at an gathering
            if (await userIsAtGathering)
            {
                var currentGathering = await user.CurrentGathering;

                // Check if user left the gathering radius
                if (!GeoLocation.AreInRange(await user.LastKnownLocation, currentGathering.Location, currentGathering.Radius))
                {
                    // Check if user is a guest or the host
                    if (currentGathering.IsHostedBy(user))
                    {
                        Log.LogWarning("Host {name} left gathering {title} area, sealing...", user.Name, currentGathering.Title);
                        // Seal the gathering if user is the host
                        await Gatherings.UpdateGatheringAsync(currentGathering.Id, new() { (nameof(CoreGathering.State), GatheringState.OngoingHidden)});
                    }
                    else
                    {
                        Log.LogWarning("Guest {name} left gathering {title} area, marking as left...", user.Name, currentGathering.Title);
                        // Leave the gathering if user is a guest
                        await Terminal.GatheringDirector.LeaveGatheringAsync(user.Id, currentGathering.Id);
                    }
                }

                // Check if user is the host
                if (currentGathering.IsHostedBy(user) && currentGathering.IsDynamic)
                {
                    // Update the position of the gathering
                    _ = Gatherings.UpdateGatheringAsync(currentGathering.Id, new() { ("Location", ((await user.LastKnownLocation).Latitude, (await user.LastKnownLocation).Longitude)) });
                }
            }
            // Check if user is on their way to an gathering
            else if (!await userIsAtGathering &&
                !nextGathering.Equals(Gathering.None))
            {
                // Check if user is host and can start gathering
                if (nextGathering.IsWaitingAuto &&
                    nextGathering.IsHostedBy(user))
                {
                    Log.LogWarning("Host {name} entered gathering {title} area, starting...", user.Name, nextGathering.Title);
                    await Terminal.GatheringDirector.StartGatheringAsync(user.Id, nextGathering.Id);
                }
                // Check if user is close enough to be arrived
                else if (nextGathering.IsOngoing &&
                    await nextGathering.IsInRange(user))
                {
                    Log.LogWarning("Guest {name} entered gathering {title} area, marking as arrived...", user.Name, nextGathering.Title);
                    await Gatherings.SetUserStateAsync(user.Id, nextGathering.Id, GatheringBond.Arrived, Time);
                }
            }
        }

		#endregion

		#region Favours

        internal async Task UpdateAllAsync(List<User> users, Func<User,List<(string Property, object Value)>> edits)
        {
            users.ForEach(user => Accounts.UpdateUserAsync(user.Id, edits(user)));
		}

        internal async Task<(GeoLocation Location, Distance Radius, int Stability)>
            RequestUserHauntAsync(User user)
        {
            var result = await Accounts.GetUserHauntAsync(user.Id);
            return (new() { Latitude = result.Latitude, Longitude = result.Longitude }, new() { Metres = result.Radius }, result.Stability);
        }

        internal async Task<(GeoLocation Location, Distance Radius)>
            RequestLastKnownUserLocationAsync(User user)
        {
            var result = await Accounts.GetRecentLocationAsync(user.Id);

            if (result == null)
            { return (GeoLocation.None, Distance.None); }

            return (new() { Latitude = result.Latitude, Longitude = result.Longitude }, new() { Metres = result.Radius });
        }

		#endregion

		#region Tools

		private async Task<User> GetUser(string phoneNumber)
        {
            User user = new(await Accounts.FindUserByPhoneNumberAsync(phoneNumber));

            // Check if user account is locked
            FailIf(user.IsLocked,
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

            FailIf(numberTaken,
                new InvalidUserException("Phone Number already registered."));
		}

        private async Task ThrowIfEmailTaken(string normalisedEmail)
        {
			bool emailTaken = false;
			try
			{
                // Throws an exception if there is no user
                await Accounts.FindUserByEmailAsync(normalisedEmail);
				emailTaken = true;
			}
			catch { }

			FailIf(emailTaken,
                new InvalidUserException("Email already registered."));
        }

		#endregion
	}
}
