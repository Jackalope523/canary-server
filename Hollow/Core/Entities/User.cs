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

        public CharacterVector Character { get; set; }

        ////////
        // Synced Properties
        //////////////////////

        public Synced<(int Postitive, int Negative)> Ratings
            => new(() => Terminal.ProfileDirector.RequestAllRatingsAsync(this));

        private Synced<(GeoLocation Location, Distance Radius)> LocationSync
            => new(() => Terminal.AccountDirector.RequestLastKnownUserLocationAsync(this));
		public Synced<GeoLocation> LastKnownLocation
            => new(async () => (await LocationSync.Value().ConfigureAwait(false)).Location);
        public Synced<Distance> LastKnownRadius
            => new(async () => (await LocationSync.Value().ConfigureAwait(false)).Radius);

        private Synced<(GeoLocation Location, Distance Radius, int Stability)> HauntSync
            => new(() => Terminal.AccountDirector.RequestUserHauntAsync(this));
        public Synced<GeoLocation> Haunt
            => new(async () => (await HauntSync.Value().ConfigureAwait(false)).Location);
        public Synced<Distance> HauntRadius
            => new(async () => (await HauntSync.Value().ConfigureAwait(false)).Radius);
        public Synced<int> HauntStability
            => new(async () => (await HauntSync.Value().ConfigureAwait(false)).Stability);

        public Synced<Event> CurrentEvent
            => new(() => Terminal.EventDirector.RequestCurrentEventForUserAsync(this));
        public Synced<List<Event>> PastEvents
            => new(() => Terminal.EventDirector.RequestPastEventsForUserAsync(this));
        public Synced<List<Event>> UpcomingEvents
            => new(() => Terminal.EventDirector.RequestUpcomingEventsForUserAsync(this));

        public Synced<List<User>> Friends
			=> new(() => Terminal.ProfileDirector.RequestFriendsAsync(this));
		public Synced<List<User>> Following
            => new(() => Terminal.ProfileDirector.RequestFollowedUsersAsync(this));
        public Synced<List<User>> FollowedBy
            => new(() => Terminal.ProfileDirector.RequestFollowersAsync(this));
        public Synced<List<User>> Blocking
            => new(() => Terminal.ProfileDirector.RequestBlockedUsersAsync(this));
        public Synced<List<User>> BlockedBy
            => new(() => Terminal.ProfileDirector.RequestUsersBlockingAsync(this));

        public Synced<List<Note>> Notes
            => new(() => Terminal.NotificationDirector.GetNotesAsync(Id));

        public Synced<List<Penalty>> Penalties
            => new(() => Terminal.DisciplineDirector.RequestPenaltiesForUserAsync(this));
        private Synced<(List<UserReport> UserReports, List<EventReport> EventReports)> ReportsSync
            => new(() => Terminal.DisciplineDirector.RequestAllReportsAsync(this));
        public Synced<List<UserReport>> Reports
            => new(async () => (await ReportsSync.Value().ConfigureAwait(false)).UserReports);
        public Synced<List<EventReport>> EventReports
            => new(async () => (await ReportsSync.Value().ConfigureAwait(false)).EventReports);

		#endregion

		#region Initialisation & Extraction

		public User() { }

        public User(UserShard fromUser)
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

        public User(UserSilhouette fromUser)
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
        }

        public User(UserProfile fromUser)
        {
            Id = fromUser.Id;
            Name = fromUser.Name;
            Reputation = fromUser.Reputation;
            NumberOfFollowers = fromUser.NumberOfFollowers;
        }

        public UserShard ToUserShard()
        {
            return new(Id, PhoneNumber, Email, Name, DateOfBirth,
                IsPhoneConfirmed, IsEmailConfirmed,
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
            if (HasAlready(DateOfBirth + (OneYear * 18))) { return false; }

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
            float modifier = MathF.Log(2.5f * timeAttended.Minutes + 3) / 70f;

            Character.MoveTowards(eventAttended.Character, modifier);
        }

        public async Task<Event> NextEvent()
        {
            return (await UpcomingEvents)[0];
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
			{ return false; }

            return true;
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
            if ((await CurrentEvent) == null)
            { return false; }

            return true;
        }

		public async Task<bool> CanView(Event @event)
		{
			// Note: This is efficient with multiple events. For multiple users, see Event.IsVisibleTo

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
			if (@event.IsOpen)
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
            return etching.UserId.Equals(Id);
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

			return UserAccountStatus.active_no_host;
        }

        public async Task<UserAccountStatus> Reported()
        {
			// Check if there are enough reports
			if ((await Reports).Count < 4)
			{ return AccountStatus; }

			// Check if there are enough reports
			if ((await Reports).Count < 6)
			{ return UserAccountStatus.active_limited; }
            
			// Check if there are enough reports
			if ((await Reports).Count < 10)
			{ return UserAccountStatus.inactive_under_review; }

            return UserAccountStatus.blacklisted;
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
