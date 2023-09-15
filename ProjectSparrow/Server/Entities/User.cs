using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Server.Boundaries;
using Server.Controls;
using Shared;

namespace Server.Entities
{
    internal class User
    {
		public Guid Id { get; init; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateOfBirth { get; init; }

        public int NumberOfFollowers { get; set; }
        public DateTimeOffset JoinDate { get; init; }

        public int Reputation { get; set; }
        public (int Postitive, int Negative) Ratings { get; set; }
        public const int MaximumReputation = 100;
        public const int ReputationPopulation = 20;
        public const float ReputationIntensity = 2.2f;

        public bool IsPhoneConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public string SecurityStamp { get; set; }
        public DateTimeOffset? LockoutDate { get; set; }
        public int AccessTries { get; set; }

        public UserAccountStatus AccountStatus { get; set; }
        public bool CanAttend => AccountStatus == UserAccountStatus.active ||
            AccountStatus == UserAccountStatus.active_no_host;
        public bool CanAttendFriends => CanAttend ||
            AccountStatus == UserAccountStatus.active_limited;
		public bool CanHost => AccountStatus == UserAccountStatus.active;
        public bool IsLocked => AccountStatus == UserAccountStatus.blacklisted;

        public Event CurrentEvent { get; set; }
        public bool IsAtEvent => CurrentEvent != null;

        public List<ThinnerUser> Following { get; set; }
        public List<ThinnerUser> Blocking { get; set; }

        public CharacterVector Character { get; set; }

        public List<UserReport> Reports { get; set; }
        public List<EventReport> EventReports { get; set; }

        public User() { }

        public User(Guid userID)
        {
            Id = userID;
        }

        public User(ThinUser fromUser)
        {
            Id = fromUser.Id;
            PhoneNumber = fromUser.PhoneNumber;
            Email = fromUser.Email;
            Name = fromUser.Name;
            DateOfBirth = fromUser.DateOfBirth;
            JoinDate = fromUser.JoinDate;
            Reputation = fromUser.Reputation;
            NumberOfFollowers = fromUser.NumberOfFollowers;
            IsPhoneConfirmed = fromUser.IsPhoneConfirmed;
            IsEmailConfirmed = fromUser.IsEmailConfirmed;
            SecurityStamp = fromUser.SecurityStamp;
            LockoutDate = fromUser.LockoutDate;
            AccessTries = fromUser.AccessTries;
            Character = new(fromUser.Character);
        }

        public User(ThinnerUser fromUser)
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
        }

        public User(ThinProfile fromUser)
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
            Reputation = fromUser.Reputation;
            NumberOfFollowers = fromUser.NumberOfFollowers;
        }

        public ThinUser ToThinUser()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed,
                SecurityStamp, LockoutDate, AccessTries, AccountStatus,
                JoinDate, Reputation, NumberOfFollowers, Character.ToCharacter());
        }

        public ThinnerUser ToThinnerUser()
        {
            return new(Id, Name);
        }

        public ThinProfile ToThinProfile()
        {
            return new(Id, Name, Reputation, NumberOfFollowers);
        }

        public async Task SyncCurrentEvent()
        {
            try
            {
                CurrentEvent = new(await EventManager.Manager.GetCurrentEventAsync(Id));
            }
            catch { }
        }

        public async Task SyncReputation()
        {
            Ratings = await AccountManager.Manager.GetAllRatingsAsync(Id);
        }

        public async Task SyncReports()
        {
            var reports = await AccountManager.Manager.GetAllReportsAsync(Id);
            Reports = reports.UserReports;
            EventReports = reports.EventReports;
        }

        public bool ValidateAndNormalise()
        {
            // Verify Phone Number
            if (!ContentValidation.TryNormalisePhoneNumber(PhoneNumber, out string normalisedPhoneNumber)) { return false; }

            // Verify Email if it exists
            if (!string.IsNullOrEmpty(Email) &&
                !ContentValidation.IsEmailValid(Email)) { return false; }

            // Verify User age
            if (DateOfBirth + TimeSpan.FromDays(365 * 18) > DateTimeOffset.UtcNow) { return false; }

            // Normalise
            Email = string.IsNullOrEmpty(Email) ? Email : Email.ToLower();
            PhoneNumber = normalisedPhoneNumber;

            return true;
        }

        public void GenerateSecurityStamp()
        {
            SecurityStamp = Convert.ToBase64String(RandomNumberGenerator.GetBytes(20));
        }

        public async Task<bool> IsFriendsWith(Guid userID)
            => await IsFriendsWith(new User(userID));

        public async Task<bool> IsFriendsWith(User otherUser)
        {
            // Check if both users are following eachother
            if (await IsFollowing(otherUser) && await otherUser.IsFollowing(this))
            { return true; }

            return false;
        }

        public async Task<bool> IsFollowing(Guid userID)
            => await IsFollowing(new User(userID));
		
        public async Task<bool> IsFollowing(User otherUser)
        {
            // Set if null
            Following ??= await AccountManager.Manager.GetFollowedUsersAsync(otherUser.Id);

			// Check if user is following target
			if (Following.Find(x => x.Id == otherUser.Id) != null)
			{ return false; }

            return true;
        }

        public async Task<bool> IsBlocking(Guid userID)
            => await IsBlocking(new User(userID));

        public async Task<bool> IsBlocking(User otherUser)
        {
            // Set if null
            Blocking ??= await AccountManager.Manager.GetBlockedUsersAsync(otherUser.Id);

			// Check if user is following target
			if (Blocking.Find(x => x.Id == otherUser.Id) != null)
			{ return false; }

            return true;
        }

        public void CalculateReputation()
        {
            int ratingDiff = Ratings.Postitive - Ratings.Negative;
            int reputationRaw = Math.Clamp(ratingDiff, -ReputationPopulation, ReputationPopulation);

            float normal = MathF.Tan(ReputationIntensity / 2) / ReputationPopulation;

            Reputation = (int) (MathF.Atan(reputationRaw * normal) * (MaximumReputation / ReputationIntensity) + (MaximumReputation / 2));
        }

        public async Task<UserAccountStatus> EventReported()
        {
            await SyncReports();

			// Check if there are enough reports
			if (EventReports.Count < 4)
			{ return AccountStatus; }

			return UserAccountStatus.active_no_host;
        }

        public async Task<UserAccountStatus> Reported()
        {
            await SyncReports();

			// Check if there are enough reports
			if (Reports.Count < 4)
			{ return AccountStatus; }

			// Check if there are enough reports
			if (Reports.Count < 6)
			{ return UserAccountStatus.active_limited; }
            
			// Check if there are enough reports
			if (Reports.Count < 10)
			{ return UserAccountStatus.inactive_under_review; }

            return UserAccountStatus.blacklisted;
        }
    }
}
