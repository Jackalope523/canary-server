using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Controls;
using Microsoft.Extensions.Hosting;
using Shared;

using static Core.Entities.Psijic;

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

        public Synced<(int Postitive, int Negative)> Ratings { get; }

        private Synced<(GeoLocation Location, Distance Radius)> LocationSync { get; }
		public Synced<GeoLocation> LastKnownLocation { get; }
        public Synced<Distance> LastKnownRadius { get; }

        private Synced<(GeoLocation Location, Distance Radius, int Stability)> HauntSync { get; }
        public Synced<GeoLocation> Haunt { get; }
        public Synced<Distance> HauntRadius { get; }
        public Synced<int> HauntStability { get; }

        public Synced<Event> CurrentEvent { get; }
        public Synced<List<Event>> PastEvents { get; }
        public Synced<List<Event>> UpcomingEvents { get; }
        public Synced<List<Event>> WatchingEvents { get; }

        public Synced<List<User>> Friends { get; }
        public Synced<List<User>> Following { get; }
        public Synced<List<User>> FollowedBy { get; }
        public Synced<List<User>> Blocking { get; }
        public Synced<List<User>> BlockedBy { get; }

        public Synced<List<Note>> Notes { get; }
        public Synced<List<Penalty>> Penalties { get; }

        private Synced<(List<UserReport> UserReports, List<EventReport> EventReports)> ReportsSync { get; }
        public Synced<List<UserReport>> Reports { get; }
        public Synced<List<EventReport>> EventReports { get; }


        #endregion

        #region Initialisation & Extraction

        public User()
        {
            Ratings = new(() => Terminal.ProfileDirector.RequestAllRatingsAsync(this));

            LocationSync = new(() => Terminal.AccountDirector.RequestLastKnownUserLocationAsync(this));
            LastKnownLocation = new(async () => (await LocationSync.Value().ConfigureAwait(false)).Location);
            LastKnownRadius = new(async () => (await LocationSync.Value().ConfigureAwait(false)).Radius);

            HauntSync = new(() => Terminal.AccountDirector.RequestUserHauntAsync(this));
            Haunt = new(async () => (await HauntSync.Value().ConfigureAwait(false)).Location);
            HauntRadius = new(async () => (await HauntSync.Value().ConfigureAwait(false)).Radius);
            HauntStability = new(async () => (await HauntSync.Value().ConfigureAwait(false)).Stability);

            CurrentEvent = new(() => Terminal.EventDirector.RequestCurrentEventForUserAsync(this));
            PastEvents = new(() => Terminal.EventDirector.RequestPastEventsForUserAsync(this));
            UpcomingEvents = new(() => Terminal.EventDirector.RequestUpcomingEventsForUserAsync(this));
            WatchingEvents = new(() => Terminal.EventDirector.RequestWatchingEventsForUserAsync(this));

            Friends = new(() => Terminal.ProfileDirector.RequestFriendsAsync(this));
            Following = new(() => Terminal.ProfileDirector.RequestFollowedUsersAsync(this));
            FollowedBy = new(() => Terminal.ProfileDirector.RequestFollowersAsync(this));
            Blocking = new(() => Terminal.ProfileDirector.RequestBlockedUsersAsync(this));
            BlockedBy = new(() => Terminal.ProfileDirector.RequestUsersBlockingAsync(this));

            Notes = new(() => Terminal.NotificationDirector.GetNotesAsync(Id));
            Penalties = new(() => Terminal.DisciplineDirector.RequestPenaltiesForUserAsync(this));

            ReportsSync = new(() => Terminal.DisciplineDirector.RequestAllReportsAsync(this));
            Reports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).UserReports);
            EventReports = new(async () => (await ReportsSync.Value().ConfigureAwait(false)).EventReports);
        }

        public User(UserShard fromUser) : this()
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

        public UserShard ToUserShard()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed, IsDeleted,
                SecurityStamp, LockoutDate, AccessTries, AccountStatus,
                JoinDate, Reputation, NumberOfFollowers, Character.ToCharacter());
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

		public bool ValidateAndNormalise()
        {
            // Verify phone number
            if (!ContentValidation.TryNormalisePhoneNumber(PhoneNumber, out string normalisedPhoneNumber)) { return false; }

            // Verify email if it exists
            if (!string.IsNullOrEmpty(Email) &&
                !ContentValidation.IsEmailValid(Email)) { return false; }

            // Verify user age
            if (HasYet(DateOfBirth + (OneYear * 18))) { return false; }

            // Normalise
            Email = string.IsNullOrEmpty(Email) ? Email : Email.ToLower();
            PhoneNumber = normalisedPhoneNumber;

            return true;
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

        public void CalculateCharacter(Event eventAttended, TimeSpan timeAttended)
        {
            // Modified by time spent
            float modifier = (float) (Math.Log10(3 * timeAttended.TotalMinutes + 3) / 15d);

            Character = Character.MoveTowards(eventAttended.Character, modifier);
        }

        public async Task<Event> NextEvent()
        {
            return (await UpcomingEvents).Count != 0 ? (await UpcomingEvents)[0] : Event.None;
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

        public async Task<bool> IsAtEvent()
        {
            if ((await CurrentEvent).Equals(Event.None))
            { return false; }

            return true;
        }

		public async Task<bool> CanView(Event @event)
		{
            // Note: This is efficient with multiple events. For multiple users, see Event.IsVisibleTo

            // Check if user is host
            if (@event.IsHostedBy(this))
            { return true; }

            // Check if event is deleted
            if (@event.IsDeleted)
            { return false; }

			// Check if user account is locked
			if (IsLocked)
			{ return false; }

			// Check if user's account is limited
			if (!CanAttend)
			{
                // User cannot join normal events
                // Check if user can join friend events and Host is friends with the user
				if (!CanAttendFriends || !await IsFriendsWith(@event.Host))
				{ return false; }
			}

            // Check if user is blocked by or blocking event host
            if (await IsBlockedBy(@event.Host) || await IsBlocking(@event.Host))
			{ return false; }

			return true;
		}

		public async Task<bool> CanJoin(Event @event)
		{
			// Check if event is joinable
			if (!@event.IsOpen)
			{ return false; }

			// Check if user can see event
			if (!await CanView(@event))
			{ return false; }

			// Check if user or user's haunt is within a reasonable distance
			if (!GeoLocation.AreInRange(await LastKnownLocation, @event.Location, @event.MaximumJoinDistance) &&
				!GeoLocation.AreInRange(await Haunt, @event.Location, @event.MaximumJoinDistance))
			{ return false; }

			return true;
		}

		public bool Etched(Etching etching)
        {
            return etching.User.Id.Equals(Id);
		}

        public async Task<bool> CanReport()
        {
            var recentReportCount = (await Reports).Count(report => After(report.ReportTime, Time - QuarterHour))
                + (await EventReports).Count(report => After(report.ReportTime, Time - QuarterHour));

            if (recentReportCount > 3)
            { return false; }

            return true;
        }

		#endregion

		#region Effects
        
        public async Task HandleHaunt()
        {
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

		public async Task<UserAccountStatus> EventReported()
        {
			// Check if there are enough reports
			if ((await EventReports).Count < 4)
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
