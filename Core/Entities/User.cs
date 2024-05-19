using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Controls;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static Core.Entities.Psijic;
using static Core.Entities.Arbiter;

namespace Core.Entities
{
    using static CoreTerminal;

    internal class User
    {

		#region Variables

        //////
        // Constants
        //////////////

        public const int MaximumReputation = 100;
        public const int ReputationPopulation = 20;
        public const float ReputationIntensity = 2.2f;

        ///////
        // Properties
        ///////////////
		
        public ulong Id { get; init; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateOfBirth { get; init; }

        public int NumberOfFollowers { get; set; }
        public DateTimeOffset JoinDate { get; init; }
        public int Reputation { get; set; }

        public bool IsPhoneConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public bool IsDeleted { get; set; }
        public string SecurityStamp { get; set; }
        public DateTimeOffset? LockoutDate { get; set; }
        public int AccessTries { get; set; }

        public UserAccountStatus AccountStatus { get; set; }
        public bool CanAttend => AccountStatus == UserAccountStatus.Active ||
            AccountStatus == UserAccountStatus.Impotent;
        public bool CanAttendFriends => CanAttend ||
            AccountStatus == UserAccountStatus.Limited;
        public bool CanHost => AccountStatus == UserAccountStatus.Active;
        public bool IsLocked => AccountStatus == UserAccountStatus.Blacklisted;

        public CharacterVector Character { get; set; }

        ////////
        // Synced Properties
        //////////////////////

        public Synced<string> Banner { get; }

        public Synced<(int Postitive, int Negative)> Ratings { get; }

        private Synced<(GeoLocation Location, Distance Radius)> LocationSync { get; }
		public Synced<GeoLocation> LastKnownLocation { get; }
        public Synced<Distance> LastKnownRadius { get; }

        private Synced<(GeoLocation Location, Distance Radius, int Stability)> HauntSync { get; }
        public Synced<GeoLocation> Haunt { get; }
        public Synced<Distance> HauntRadius { get; }
        public Synced<int> HauntStability { get; }

        public Synced<Gathering> CurrentGathering { get; }
        public Synced<List<Gathering>> PastGatherings { get; }
        public Synced<List<Gathering>> UpcomingGatherings { get; }
        public Synced<List<Gathering>> SurveyingGatherings { get; }

        public Synced<List<User>> Friends { get; }
        public Synced<List<User>> Following { get; }
        public Synced<List<User>> FollowedBy { get; }
        public Synced<List<User>> Blocking { get; }
        public Synced<List<User>> BlockedBy { get; }

        public Synced<List<NoteShard>> Notes { get; }
        public Synced<List<PenaltyShard>> Penalties { get; }

        private Synced<(List<UserReport> UserReports, List<GatheringReport> GatheringReports)> ReportsSync { get; }
        public Synced<List<UserReport>> Reports { get; }
        public Synced<List<GatheringReport>> GatheringReports { get; }


        #endregion

        #region Initialisation & Extraction

        public User()
        {
            Banner = new(() => Terminal.BannerDirector.RequestUserBannerAsync(this));

            Ratings = new(() => Terminal.ProfileDirector.RequestAllRatingsAsync(this));

            LocationSync = new(() => Terminal.AccountDirector.RequestLastKnownUserLocationAsync(this));
            LastKnownLocation = new(async () => (await LocationSync.Value().ConfigureAwait(false)).Location);
            LastKnownRadius = new(async () => (await LocationSync.Value().ConfigureAwait(false)).Radius);

            HauntSync = new(() => Terminal.AccountDirector.RequestUserHauntAsync(this));
            Haunt = new(async () => (await HauntSync.Value().ConfigureAwait(false)).Location);
            HauntRadius = new(async () => (await HauntSync.Value().ConfigureAwait(false)).Radius);
            HauntStability = new(async () => (await HauntSync.Value().ConfigureAwait(false)).Stability);

            CurrentGathering = new(() => Terminal.GatheringDirector.RequestCurrentGatheringForUserAsync(this));
            PastGatherings = new(() => Terminal.GatheringDirector.RequestPastGatheringsForUserAsync(this));
            UpcomingGatherings = new(() => Terminal.GatheringDirector.RequestUpcomingGatheringsForUserAsync(this));
            SurveyingGatherings = new(() => Terminal.GatheringDirector.RequestSurveyingGatheringsForUserAsync(this));

            Friends = new(() => Terminal.ProfileDirector.RequestFriendsAsync(this));
            Following = new(() => Terminal.ProfileDirector.RequestFollowedUsersAsync(this));
            FollowedBy = new(() => Terminal.ProfileDirector.RequestFollowersAsync(this));
            Blocking = new(() => Terminal.ProfileDirector.RequestBlockedUsersAsync(this));
            BlockedBy = new(() => Terminal.ProfileDirector.RequestUsersBlockingAsync(this));

            Notes = new(() => Terminal.NotificationDirector.GetNotesAsync(Id));
            Penalties = new(() => Terminal.DisciplineDirector.RequestPenaltiesForUserAsync(this));

            ReportsSync = new(() => Terminal.DisciplineDirector.RequestAllReportsAsync(this));
            Reports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).UserReports);
            GatheringReports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).GatheringReports);
        }

        public User(CoreUser fromUser) : this()
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
            IsDeleted = fromUser.IsPendingDeletion;
            SecurityStamp = fromUser.SecurityStamp;
            LockoutDate = fromUser.LockoutDate;
            AccessTries = fromUser.AccessTries;
            AccountStatus = fromUser.AccountStatus;
            Character = new(fromUser.Character);
        }

        public User(UserSilhouette fromUser) : this()
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
        }

        public User(UserProfile fromUser) : this()
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
            Reputation = fromUser.Reputation;
            NumberOfFollowers = fromUser.NumberOfFollowers;
        }

        public CoreUser ToCoreUser()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed, IsDeleted,
                SecurityStamp, LockoutDate, AccessTries, AccountStatus,
                JoinDate, Reputation, NumberOfFollowers, Character.ToCharacter());
        }

        public UserShard ToUserShard()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                Reputation, NumberOfFollowers);
        }

        public UserSilhouette ToUserSilhouette()
        {
            return new(Id, Name);
        }

        public UserProfile ToUserProfile()
        {
            return new(Id, Name, Reputation, NumberOfFollowers);
        }

		#endregion

		#region Composition

		public bool ValidateAndNormalise(out string issues)
        {
            issues = "";

            // Verify phone number
            if (!ContentValidation.TryNormalisePhoneNumber(PhoneNumber, out string normalisedPhoneNumber))
            { issues += "Invalid phone number. "; }

            // Verify email if it exists
            if (!string.IsNullOrEmpty(Email) &&
                !ContentValidation.IsEmailValid(Email)) { issues += "Invalid email. "; }

            // Verify user age
            if (HasYet(DateOfBirth + (OneYear * 18))) { issues += "User is too young. "; }

            // Normalise
            Email = string.IsNullOrEmpty(Email) ? Email : Email.ToLower();
            PhoneNumber = normalisedPhoneNumber;

            return issues.Equals("");
        }

        public void GenerateSecurityStamp()
        {
            SecurityStamp = Convert.ToBase64String(RandomNumberGenerator.GetBytes(20));
        }

		public async Task CalculateReputation()
        {
            _ = (Penalties.Sync(), Ratings.Sync());

            // Get all recent penalties
            var penalties = (await Penalties).Where(penalty => HasYet(penalty.TimeOfPenalty + OneYear)).ToList();
            var ratings = await Ratings;
            int ratingDiff = ratings.Postitive - ratings.Negative - penalties.Count/2;
            int reputationRaw = Math.Clamp(ratingDiff, -ReputationPopulation, ReputationPopulation);

            float normal = MathF.Tan(ReputationIntensity / 2) / ReputationPopulation;

            Reputation = (int) (MathF.Atan(reputationRaw * normal) * (MaximumReputation / ReputationIntensity) + (MaximumReputation / 2));
        }

        public void CalculateCharacter(Gathering gatheringAttended, TimeSpan timeAttended)
        {
            // Modified by time spent
            float modifier = (float) (Math.Log10(3 * timeAttended.TotalMinutes + 3) / 15d);

            Character = Character.MoveTowards(gatheringAttended.Character, modifier);
        }

        public async Task<Gathering> NextGathering()
        {
            return (await UpcomingGatherings).Count != 0 ? (await UpcomingGatherings)[0] : Gathering.None;
        }

		#endregion

		#region Checks

        public async Task<bool> IsFriendsWith(User otherUser)
		{
			// Check if users are friends
			if ((await Friends).Contains(otherUser))
            { return true; }

            return false;
        }

        public async Task<bool> IsFollowing(User otherUser)
        {
			// Check if user is following target
			if ((await Following).Contains(otherUser))
			{ return true; }

            return false;
        }

        public async Task<bool> IsBlocking(User otherUser)
        {
			// Check if user is blocking target
			if ((await Blocking).Contains(otherUser))
			{ return true; }

            return false;
        }

		public async Task<bool> IsBlockedBy(User otherUser)
		{
			// Check if user is blocked by target
			if ((await BlockedBy).Contains(otherUser))
			{ return true; }

			return false;
		}

        public async Task<bool> IsAtGathering()
        {
            if ((await CurrentGathering).Equals(Gathering.None))
            { return false; }

            return true;
        }

		public async Task<bool> CanView(Gathering @gathering)
		{
            // Note: This is efficient with multiple gatherings. For multiple users, see Gathering.IsVisibleTo

            // Check if user is host
            if (@gathering.IsHostedBy(this))
            { return true; }

            // Check if gathering is deleted
            if (@gathering.IsDeleted)
            { return false; }

			// Check if user account is locked
			if (IsLocked)
			{ return false; }

			// Check if user's account is limited
			if (!CanAttend)
			{
                // User cannot join normal gatherings
                // Check if user can join friend gatherings and Host is friends with the user
				if (!CanAttendFriends || !await IsFriendsWith(@gathering.Host))
				{ return false; }
			}

            // Check if user is blocked by or blocking gathering host
            if (await IsBlockedBy(@gathering.Host) || await IsBlocking(@gathering.Host))
			{ return false; }

			return true;
		}

		public async Task<bool> CanJoin(Gathering @gathering)
		{
			// Check if gathering is joinable
			if (!@gathering.IsOpen)
			{ return false; }

			// Check if user can see gathering
			if (!await CanView(@gathering))
			{ return false; }

			// Check if user or user's haunt is within a reasonable distance
			if (!GeoLocation.AreInRange(await LastKnownLocation, @gathering.Location, @gathering.MaximumJoinDistance) &&
				!GeoLocation.AreInRange(await Haunt, @gathering.Location, @gathering.MaximumJoinDistance))
			{ return false; }

			return true;
		}

        public async Task CanEtch(Gathering @gathering)
		{
			// Verify etching is not before gathering starting or user is host
			Try(HasAlready(@gathering.StartTime) || @gathering.IsModifiableBy(this),
				new InvalidGatheringException("Gathering has yet to start."));

			// Verify user can etch into the gathering
			Try(await @gathering.WasAttendedBy(this) || @gathering.IsModifiableBy(this),
				new InvalidGatheringException("User did not attend gathering."));

			// Verify etching is added before gathering is closed
			Try(@gathering.IsActive,
				new InvalidGatheringException("Gathering has already ended."));
		}

		public bool Etched(EtchingShard etching)
        {
            return etching.User.Id.Equals(Id);
		}

        public async Task<bool> CanReport()
        {
            var recentReportCount = (await Reports).Count(report => After(report.ReportTime, Time - QuarterHour))
                + (await GatheringReports).Count(report => After(report.ReportTime, Time - QuarterHour));

            if (recentReportCount > 3)
            { return false; }

            return true;
        }

		#endregion

		#region Effects
        
        public async Task HandleHaunt()
        {
            // Check if user has a haunt and recent location
            if (!(await Haunt).Exists || !(await LastKnownLocation).Exists)
            {
                Haunt.Set(await LastKnownLocation);
                HauntStability.Set(1);
                return;
            }

            // Check if recent location is within haunt area
            if (GeoLocation.DistanceBetween(await Haunt, await LastKnownLocation) < await HauntRadius)
            {
                HauntStability.Set(HauntStability + 1);
            }
            else
            {
                HauntStability.Set(HauntStability - 1);

                // If our haunt is unstable, move it
                if (await HauntStability < 0)
                {
                    HauntStability.Set(0);
                    Haunt.Set(await LastKnownLocation);
                }
            }
        }

        public async Task Penalised()
            => await CalculateReputation();

		public async Task<UserAccountStatus> GatheringReported()
        {
			// Check if there are enough reports
			if ((await GatheringReports).Count < 4)
			{ return AccountStatus; }

			return UserAccountStatus.Impotent;
        }

        public async Task<UserAccountStatus> Reported()
        {
			// Check if there are enough reports
			if ((await Reports).Count < 4)
			{ return AccountStatus; }

			// Check if there are enough reports
			if ((await Reports).Count < 6)
			{ return UserAccountStatus.Limited; }
            
			// Check if there are enough reports
			if ((await Reports).Count < 10)
			{ return UserAccountStatus.Suspended; }

            return UserAccountStatus.Blacklisted;
        }

		#endregion

		#region Actions

        public async Task PostNote(User notifier, string message, string action)
        {
            await Terminal.NotificationDirector.PostNoteAsync(this, notifier,
                message, action);
        }

		public async Task Notify(string title, string message)
        {
            await Terminal.NotificationDirector.NotifyUserAsync(this, title, message);
        }

        public async Task NotifyFollowers(string title, string message)
        {
            (await FollowedBy).ForEach(follower => _ = follower.Notify(title, message));
        }

		#endregion

		#region Dissimilation

		public override bool Equals(object obj)
		{
			return obj is User other && Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion
	}
}
