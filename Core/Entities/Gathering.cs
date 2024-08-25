using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;
using Microsoft.Extensions.Logging;

namespace Core.Entities
{
    using static CoreTerminal;

    internal class Gathering
    {
		#region Variables

		//////
		// Constants
		//////////////

		public const int MaximumNameLength = 50;
        public const int MaximumDescLength = 400;
        public const int MaximumLocationLength = 80;

        public static readonly Distance MaximumJoinDistance = new() { Kilometres = 200 };
        public static readonly Distance ArrivalDistance = new() { Metres = 75 };
        public static readonly TimeSpan MaximumSnapshotLateness = OneDay;
        public static readonly TimeSpan MaximumEarlyBirdStart = TimeSpan.FromMinutes(20);
        public static readonly TimeSpan MaximumAutoStart = TimeSpan.FromMinutes(5);

        public static Gathering None
            => new() { Id = 0, Exists = false };

		///////
		// Properties
		///////////////

		public ulong Id { get; init; }
        public User Host { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CharacterVector Character { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public GeoLocation Location { get; set; }
        public string FriendlyLocation { get; set; }
        public Distance Radius { get; set; }
        public bool IsDynamic { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public GatheringState State { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }
        public bool IsDeleted { get; set; }
        public int NumberOfGuests { get; set; }
        public float RelativeAngle { get; set; } = 0;

        public bool IsWaiting
            => State.Equals(GatheringState.Upcoming) &&
                HasAlready(StartTime - MaximumEarlyBirdStart);
        public bool IsWaitingAuto
            => State.Equals(GatheringState.Upcoming) &&
                HasAlready(StartTime - MaximumAutoStart);
        public bool IsOpen
            => State.Equals(GatheringState.Upcoming) ||
                State.Equals(GatheringState.Open);
        public bool IsOngoing
            => State.Equals(GatheringState.Open) ||
                State.Equals(GatheringState.Sealed);
        public bool IsActive
            => !EndTime.HasValue ||
                HasYet(EndTime.Value + MaximumSnapshotLateness);
        public bool IsTerminated
            => EndTime.HasValue;

        public bool Exists { get; set; } = true;

        ////////
        // Synced Properties
        //////////////////////

        public Synced<List<(User User, GatheringBond State)>> AllUsers { get; }
        public Synced<List<User>> Surveying { get; }
        public Synced<List<User>> Guests { get; }
        public Synced<List<User>> Arrived { get; }
        public Synced<List<User>> Left { get; }
        public Synced<List<User>> Kicked { get; }

        public Synced<List<(DateTimeOffset Joined, DateTimeOffset? Left, User User)>> GuestHistory { get; }

        public Synced<List<GatheringReport>> GatheringReports { get; }

        public Synced<List<SnapshotShard>> Snapshots { get; }

        #endregion

        #region Initialisation & Extraction

        public Gathering()
        {
            AllUsers = new(() => Terminal.GatheringDirector.RequestAllUsersFromGatheringAsync(this));
            Surveying = new(async () => (await AllUsers.Value().ConfigureAwait(false)).FindAll(user => user.State.Equals(GatheringBond.Watching)).ConvertAll(user => user.User));
            Guests = new(async () => (await AllUsers.Value().ConfigureAwait(false)).FindAll(user => user.State.Equals(GatheringBond.Guest)).ConvertAll(user => user.User));
            Arrived = new(async () => (await AllUsers.Value().ConfigureAwait(false)).FindAll(user => user.State.Equals(GatheringBond.Arrived)).ConvertAll(user => user.User));
            Left = new(async () => (await AllUsers.Value().ConfigureAwait(false)).FindAll(user => user.State.Equals(GatheringBond.Left)).ConvertAll(user => user.User));
            Kicked = new(async () => (await AllUsers.Value().ConfigureAwait(false)).FindAll(user => user.State.Equals(GatheringBond.Kicked)).ConvertAll(user => user.User));

            GuestHistory = new(() => Terminal.GatheringDirector.RequestGuestHistoryAsync(this));
            GatheringReports = new(() => Terminal.DisciplineDirector.RequestGatheringReportsAsync(this));
            Snapshots = new(() => Terminal.SnapshotDirector.RequestGatheringSnapshotsAsync(this));
        }

        public Gathering(CoreGathering fromGathering) : this()
        {
            Id = fromGathering.Id;
            Host = new(fromGathering.Host);
            Name = fromGathering.Name;
            Description = fromGathering.Description;
            StartTime = fromGathering.StartTime;
            Location = new()
                { Latitude = fromGathering.Latitude, Longitude = fromGathering.Longitude };
            FriendlyLocation = fromGathering.FriendlyLocation;
            EndTime = fromGathering.TimeEnded;
            State = fromGathering.State;
            GroupMinimum = fromGathering.GroupMinimum;
            GroupMaximum = fromGathering.GroupMaximum;
            Character = new(fromGathering.Character);
            Radius = new() { Kilometres = fromGathering.Radius };
            IsDynamic = fromGathering.IsDynamic;
            IsDeleted = fromGathering.IsPendingDeletion;
            NumberOfGuests = fromGathering.NumberOfGuests;
        }

        public Gathering(GatheringShard fromGathering) : this()
        {
            Id = fromGathering.Id;
            Host = new(fromGathering.Host);
            Name = fromGathering.Name;
            Description = fromGathering.Description;
            StartTime = fromGathering.StartTime;
            Location = new()
                { Latitude = fromGathering.Latitude, Longitude = fromGathering.Longitude };
            EndTime = fromGathering.TimeEnded;
            State = fromGathering.State;
            GroupMinimum = fromGathering.GroupMinimum;
            GroupMaximum = fromGathering.GroupMaximum;
            Radius = new() { Kilometres = fromGathering.Radius };
            NumberOfGuests = fromGathering.NumberOfGuests;
        }

        public CoreGathering ToCoreGathering()
        {
            return new(Id, Host.ToUserShard(), Name, Description,
                StartTime, Location.Latitude, Location.Longitude, FriendlyLocation,
                EndTime, State, GroupMinimum, GroupMaximum, Character.ToCharacter(),
                Radius.Kilometres, IsDynamic, IsDeleted, NumberOfGuests);
        }

        public GatheringShard ToGatheringShard()
        {
            return new(Id, Host.ToUserShard(), Name, Description,
                StartTime, Location.Latitude, Location.Longitude, FriendlyLocation,
                EndTime, State, GroupMinimum, GroupMaximum,
                Radius.Kilometres, NumberOfGuests, RelativeAngle);
        }

        public GatheringShard ToGatheringShard(User relativeUser)
        {
            return new(Id, Host.ToUserShard(), Name, Description,
                StartTime, Location.Latitude, Location.Longitude, FriendlyLocation,
                EndTime, State, GroupMinimum, GroupMaximum,
                Radius.Kilometres, NumberOfGuests,
                CharacterVector.AngleBetweenAffected(relativeUser.Character, Character));
        }

        public GatheringHeader ToGatheringHeader(DateTimeOffset lastActiveTime)
        {
            return new(Id, Name, IsOngoing ? StartTime : EndTime.Value, IsActive, lastActiveTime, FriendlyLocation);
        }

		#endregion

		#region Composition

		public bool ValidateAndNormalise(out string issues)
        {
            issues = "";

            // Sanitise User content
            Name = ContentValidation.NormaliseText(Name, MaximumNameLength);
            Description = ContentValidation.NormaliseText(Description, MaximumDescLength);
            FriendlyLocation = ContentValidation.NormaliseText(FriendlyLocation, MaximumLocationLength);

            // Verify Gathering is now or in the future
            if (HappenedBefore(StartTime, Time - MaximumEarlyBirdStart)) { issues += "Gathering is in the past. "; }

            // If in the past, make it now
            if (HappenedBefore(StartTime, Time)) { StartTime = Time; }

            // Verify Gathering is within a reasonable time
            if (After(StartTime, Time + OneWeek)) { issues += "Gathering is too far in the future. "; }

            // Verify group bounds
            if (GroupMaximum != 0 &&
                (GroupMaximum <= GroupMinimum ||
                GroupMaximum < 4)) { issues += "Gathering group bounds invalid. "; }

            return issues.Equals("");
        }

        public async Task<List<(User User, GatheringBond State)>> GetCompanionsOf(User user)
        {
            List<(User User, GatheringBond State)> companions = new();

            foreach (var userDetails in await AllUsers)
            {
                if (await user.IsCompanionsWith(userDetails.User))
                {
                    companions.Add(userDetails);
                }
            }

            return companions;
        }

        public async Task<List<SnapshotShard>> GetSnapshotsOf(User user)
        {
            return (await Snapshots).Where(snapshot => snapshot.User.Id.Equals(user.Id)).ToList();
        }

		#endregion

		#region Checks

		public async Task<bool> IsVisibleTo(User user)
		{
			// Note: This is efficient with multiple users. For multiple gatherings, see User.CanView

            // Check if user is host
            if (IsHostedBy(user))
            { return true; }

            // Check if gathering is deleted
            if (IsDeleted)
            { return false; }

			// Check if user account is locked
			if (user.IsLocked)
            { return false; }

			// Check if user's account is limited
			if (!user.CanAttend)
			{
				// User cannot join normal gatherings
                // Check if user can join companion gatherings and Host is companions with the user
				if (!(user.CanAttendCompanions && await Host.IsCompanionsWith(user)))
				{ return false; }
			}

			// Check if user is blocked by or blocking gathering host
			if (await Host.IsBlockedBy(user) || await Host.IsBlocking(user))
			{ return false; }

            return true;
		}

        public async Task<bool> IsJoinableBy(User user)
        {
            // Check if gathering is joinable
            if (!IsOpen)
            { return false; }

            // Check if user can see gathering
            if (!await IsVisibleTo(user))
            { return false; }

            // Check if user is kicked from gathering
            if ((await Kicked).Contains(user))
            { return false; }

            /*
            // Check if user or user's haunt is within a reasonable distance
            if (!GeoLocation.AreInRange(await user.LastKnownLocation, Location, MaximumJoinDistance) &&
                !GeoLocation.AreInRange(await user.Haunt, Location, MaximumJoinDistance))
            { return false; }
            */

            return true;
        }

        public bool IsModifiableBy(User user)
        {
			// Check if user is gathering host
			if (Host.Id.Equals(user.Id))
			{ return true; }

			return false;
        }

        public bool IsHostedBy(User user)
        {
			// Check if user is gathering host
			if (Host.Equals(user))
			{ return true; }

			return false;
        }

        public async Task<bool> HasUserRelationship(User user)
        {
            // Check if user has interacted with gathering
            return IsHostedBy(user) || (await AllUsers).Exists(x => x.User.Id == user.Id);
        }

        public async Task<bool> HasOnGuestList(User user)
        {
            // Check if user is affiliated with the gathering
            return (await Guests).Contains(user) || await WasAttendedBy(user);
        }

        public async Task<bool> WasAttendedBy(User user)
        {
            // Check if user is or was on the guest list
            return (await Arrived).Contains(user) || (await Left).Contains(user);
		}
        
        public async Task<bool> IsInRange(User user)
            => GeoLocation.AreInRange(Location, await user.LastKnownLocation, ArrivalDistance);

        public async Task<bool> IsStartable()
        {
            // Check if gathering has not yet started
            if (!IsWaiting)
            { return false; }

            // Check if host is within range
            if (!await IsInRange(Host))
            { return false; }

            return true;
        }

        public bool IsTerminable()
        {
            // Ensure gathering is ongoing
            if (!IsOngoing)
            { return false; }

            return true;
        }

        public bool IsDeletable()
        {
            // Ensure gathering has not already occurred
            if (IsOngoing || IsTerminated)
            { return false; }

            return true;
        }

		#endregion

		#region Effects

        public async Task Started()
        {
            _ = NotifyActive($"{Name}", "Gathering is active!");
        }

        public async Task<List<User>> Ended()
        {
            List<User> updatedGuests = new();

            // Update all participants' vectors and notify
			foreach ((var joined, var left, var guest) in await GuestHistory)
			{
				guest.CalculateCharacter(this, left.Value - joined);

                updatedGuests.Add(guest);

				// Notify of gathering ending
				_ = guest.Notify($"{Name}", $"Gathering has concluded.");
			}

            return updatedGuests;
		}

        public async Task Taken(User user)
        {
            // Verify snapshot is not before gathering starting or user is host
            Verify(HasAlready(StartTime) || IsModifiableBy(user),
                new InvalidGatheringException("Gathering has yet to start."));

            // Verify user can etch into the gathering
            Verify(await WasAttendedBy(user) || IsModifiableBy(user),
                new InvalidGatheringException("User did not attend gathering."));

            // Verify snapshot is added before gathering is closed
            Verify(IsActive,
                new InvalidGatheringException("Gathering has already ended."));
		}

		public async Task<bool> Reported()
        {
            // Check if there are enough reports
            if ((await GatheringReports).Count < 3)
            { return false; }

            return true;
        }

        #endregion

        #region Actions

        public async Task NotifyActive(string title, string message)
        {
            foreach (var user in (await Guests).Concat(await Arrived))
            {
                if (IsHostedBy(user))
                { continue; }

                _ = user.Notify(title, message);
            }
        }

        public async Task NotifyGuests(string title, string message)
        {
            foreach (var guest in await Arrived)
            {
                if (IsHostedBy(guest))
                { continue; }

                _ = guest.Notify(title, message);
            }
        }

		#endregion

		#region Dissimilation

		public override bool Equals(object obj)
		{
			return obj is Gathering other &&
                Exists == other.Exists &&
                Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion
	}
}
