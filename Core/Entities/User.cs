using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Boundaries;

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

        public static User Hollow
            => new() { Id = 0 };

        ///////
        // Properties
        ///////////////
		
        public ulong Id { get; init; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Pseudonym { get; set; }
        public DateTimeOffset DateOfBirth { get; init; }

        public DateTimeOffset JoinDate { get; init; }
        public int Reputation { get; set; }

        public bool IsPhoneConfirmed { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public bool IsDeleted { get; set; }
        public string SecurityStamp { get; set; }
        public DateTimeOffset? LockoutDate { get; set; }
        public int AccessTries { get; set; }
        public DateTimeOffset TimeOfUserAgreement { get; set; }
        public Guid NotificationId { get; set; }

        public UserAccountStatus AccountStatus { get; set; }
        public bool CanAttend => AccountStatus == UserAccountStatus.Active ||
            AccountStatus == UserAccountStatus.Impotent;
        public bool CanAttendCompanions => CanAttend ||
            AccountStatus == UserAccountStatus.Limited;
        public bool CanHost => AccountStatus == UserAccountStatus.Active;
        public bool IsLocked => AccountStatus == UserAccountStatus.Blacklisted;

        public CharacterVector Character { get; set; }

        ////////
        // Synced Properties
        //////////////////////

        public Synced<Banner> Banner { get; }

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

        public Synced<List<User>> Companions { get; }
        public Synced<List<User>> Appreciating { get; }
        public Synced<List<User>> AppreciatedBy { get; }
        public Synced<List<User>> Blocking { get; }
        public Synced<List<User>> BlockedBy { get; }

        public Synced<List<TelegramShard>> Notes { get; }
        public Synced<List<PenaltyShard>> Penalties { get; }

        private Synced<(List<UserReport> UserReports, List<GatheringReport> GatheringReports, List<SnapshotReport> SnapshotReports)> ReportsSync { get; }
        public Synced<List<UserReport>> Reports { get; }
        public Synced<List<GatheringReport>> GatheringReports { get; }
        public Synced<List<SnapshotReport>> SnapshotReports { get; }


        #endregion

        #region Initialisation & Extraction

        public User()
        {
            Banner = new(() => Terminal.BannerDirector.RequestUserBannerAsync(this));

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

            Companions = new(() => Terminal.NestDirector.RequestCompanionsAsync(this));
            Appreciating = new(() => Terminal.NestDirector.RequestAppreciatedUsersAsync(this));
            AppreciatedBy = new(() => Terminal.NestDirector.RequestAppreciateersAsync(this));
            Blocking = new(() => Terminal.NestDirector.RequestBlockedUsersAsync(this));
            BlockedBy = new(() => Terminal.NestDirector.RequestUsersBlockingAsync(this));

            Notes = new(() => Terminal.NotificationDirector.GetTelegramsAsync(Id));
            Penalties = new(() => Terminal.DisciplineDirector.RequestPenaltiesForUserAsync(this));

            ReportsSync = new(() => Terminal.DisciplineDirector.RequestAllReportsAsync(this));
            Reports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).UserReports);
            GatheringReports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).GatheringReports);
            SnapshotReports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).SnapshotReports);
        }

        public User(CoreUser fromUser) : this()
        {
            Id = fromUser.Id;
            PhoneNumber = fromUser.PhoneNumber;
            Email = fromUser.Email;
            Name = fromUser.Name;
            Pseudonym = fromUser.Pseudonym;
            DateOfBirth = fromUser.DateOfBirth;
            JoinDate = fromUser.JoinDate;
            Reputation = fromUser.Reputation;
            IsPhoneConfirmed = fromUser.IsPhoneConfirmed;
            IsEmailConfirmed = fromUser.IsEmailConfirmed;
            IsDeleted = fromUser.IsPendingDeletion;
            SecurityStamp = fromUser.SecurityStamp;
            LockoutDate = fromUser.LockoutDate;
            AccessTries = fromUser.AccessTries;
            AccountStatus = fromUser.AccountStatus;
            Character = new(fromUser.Character);
            TimeOfUserAgreement = fromUser.TimeOfUserAgreement;
            NotificationId = fromUser.NotificationId;
        }

        public User(UserShard fromUser) : this()
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
        }

        public CoreUser ToCoreUser()
        {
            return new(Id, PhoneNumber, Email, Name, Pseudonym, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed, IsDeleted,
                SecurityStamp, LockoutDate, AccessTries, AccountStatus,
                JoinDate, Reputation,
                Character.ToCharacter(), TimeOfUserAgreement,
                NotificationId);
        }

        public AccountShard ToAccountShard()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed, AccountStatus,
                JoinDate, TimeOfUserAgreement, NotificationId);
        }

        public UserShard ToUserShard()
        {
            return new(Id, Name);
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

        public int GetAge()
        {
            int age = Time.Year - DateOfBirth.Year;

            if (Time < DateOfBirth.AddYears(age))
            {
                age--;
            }

            return age;
        }

        public void GenerateSecurityStamp()
        {
            SecurityStamp = Convert.ToBase64String(RandomNumberGenerator.GetBytes(20));
        }

		public async Task CalculateReputation()
        {
            _ = (Penalties.Sync(), AppreciatedBy.Sync());

            // Get all recent penalties
            var penalties = (await Penalties).Where(penalty => HasYet(penalty.TimeOfPenalty + OneYear)).ToList();
            int appreciations = (await AppreciatedBy).Count;
            int reputationRaw = Math.Clamp(appreciations, -ReputationPopulation, ReputationPopulation);

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
            var upcoming = await UpcomingGatherings;
            upcoming.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
            return upcoming.Count != 0 ? upcoming.First() : Gathering.None;
        }

		#endregion

		#region Checks

        public async Task<bool> IsCompanionsWith(User otherUser)
		{
			// Check if users are companions
			if ((await Companions).Contains(otherUser))
            { return true; }

            return false;
        }

        public async Task<bool> IsAppreciating(User otherUser)
        {
			// Check if user is appreciating target
			if ((await Appreciating).Contains(otherUser))
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

		public async Task<bool> CanView(Gathering gathering)
		{
            // Note: This is efficient with multiple gatherings. For multiple users, see Gathering.IsVisibleTo

            // Check if user is host
            if (gathering.IsHostedBy(this))
            { return true; }

            // Check if gathering is deleted
            if (gathering.IsDeleted)
            { return false; }

			// Check if user account is locked
			if (IsLocked)
			{ return false; }

			// Check if user's account is limited
			if (!CanAttend)
			{
                // User cannot join normal gatherings
                // Check if user can join companion gatherings and Host is companions with the user
				if (!(CanAttendCompanions && await IsCompanionsWith(gathering.Host)))
				{ return false; }
			}

            // Check if user is blocked by or blocking gathering host
            if (await IsBlockedBy(gathering.Host) || await IsBlocking(gathering.Host))
			{ return false; }

			return true;
		}

		public async Task<bool> CanJoin(Gathering gathering)
		{
			// Check if gathering is joinable
			if (!gathering.IsOpen)
			{ return false; }

			// Check if user can see gathering
			if (!await CanView(gathering))
			{ return false; }

            // Check if user is kicked from gathering
            if ((await gathering.Kicked).Contains(this))
            { return false; }

            /*
			// Check if user or user's haunt is within a reasonable distance
			if (!GeoLocation.AreInRange(await LastKnownLocation, gathering.Location, gathering.MaximumJoinDistance) &&
				!GeoLocation.AreInRange(await Haunt, gathering.Location, gathering.MaximumJoinDistance))
			{ return false; }
            */

            return true;
		}

        public async Task CanEtch(Gathering gathering)
		{
			// Verify snapshot is not before gathering starting or user is host
			Verify(HasAlready(gathering.StartTime) || gathering.IsModifiableBy(this),
				new InvalidGatheringException("Gathering has yet to start."));

			// Verify user can etch into the gathering
			Verify(await gathering.WasAttendedBy(this) || gathering.IsModifiableBy(this),
				new InvalidGatheringException("User did not attend gathering."));

			// Verify snapshot is added before gathering is closed
			Verify(gathering.IsActive,
				new InvalidGatheringException("Gathering has already ended."));
		}

		public bool Taken(SnapshotShard snapshot)
        {
            return snapshot.User.Id.Equals(Id);
		}

        public async Task<bool> CanReport()
        {
            var recentReportCount = (await Reports).Count(report => After(report.ReportTime, Time - FifteenMinutes))
                + (await GatheringReports).Count(report => After(report.ReportTime, Time - FifteenMinutes));

            if (recentReportCount > 3)
            { return false; }

            return true;
        }

        public async Task<bool> CanAppreciate(User target)
        {
            var haveMutualGatheringSync = Terminal.NestDirector.RequestAttendedMutualGatheringAsync(this, target);
            bool blockAppreciate = await IsBlocking(target) || await IsBlockedBy(target);

            return !blockAppreciate && await haveMutualGatheringSync;
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
            var currentStatus = AccountStatus;
            UserAccountStatus nextStatus;

			// Check if there are enough reports
			if ((await Reports).Count < 4)
			{ return AccountStatus; }
			else if ((await Reports).Count < 6)
			{ nextStatus = UserAccountStatus.Limited; }
            else if ((await Reports).Count < 10)
			{ nextStatus = UserAccountStatus.Suspended; }
            else
            { nextStatus = UserAccountStatus.Blacklisted; }

            // Notify user of change
            if (!currentStatus.Equals(nextStatus))
            { _ = PostTelegram(Hollow, TelegramMessage.AccountStatusChanged);  }

            return nextStatus;
        }

		#endregion

		#region Actions

        public async Task PostTelegram(User notifier, TelegramMessage message, string context = "")
        {
            await Terminal.NotificationDirector.PostTelegramAsync(this, notifier,
                message, context);
        }

		public async Task Notify(string title, string message)
        {
            await Terminal.NotificationDirector.NotifyUserAsync(this, title, message);
        }

        public async Task NotifyAppreciateers(string title, string message)
        {
            (await AppreciatedBy).ForEach(appreciateer => _ = appreciateer.Notify(title, message));
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
