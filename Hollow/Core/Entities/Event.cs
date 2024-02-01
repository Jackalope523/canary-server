using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared;
using Core.Boundaries;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;

namespace Core.Entities
{
    using static CoreTerminal;

    internal class Event
    {
		#region Variables

		//////
		// Constants
		//////////////

		public const int MaximumNameLength = 50;
        public const int MaximumDescLength = 400;

        public readonly Distance MaximumJoinDistance = new() { Kilometres = 200 };
        public readonly Distance ArrivalDistance = new() { Metres = 75 };
        public readonly TimeSpan MaximumEtchingLateness = OneDay;

        public static Event None
            => new() { Id = 0 };

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
        public Distance Radius { get; set; }
        public bool IsDynamic { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public EventState State { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }

        public bool IsWaiting
            => State.Equals(EventState.Upcoming) &&
                HasAlready(StartTime);
        public bool IsOpen
            => State.Equals(EventState.Upcoming) ||
                State.Equals(EventState.Open);
        public bool IsOngoing
            => State.Equals(EventState.Open) ||
                State.Equals(EventState.Sealed);
        public bool IsActive
            => !EndTime.HasValue ||
                HasYet(EndTime.Value + MaximumEtchingLateness);

		////////
		// Synced Properties
		//////////////////////

		public Synced<List<(User User, EventBond State)>> AllUsers
            => new(() => Terminal.EventDirector.RequestAllUsersFromEventAsync(this));
        public Synced<List<User>> Watching
            => new(async () => (await AllUsers.Value()).FindAll(user => user.State.Equals(EventBond.Watching)).ConvertAll(user => user.User));
		public Synced<List<User>> Guests
            => new(async () => (await AllUsers.Value()).FindAll(user => user.State.Equals(EventBond.Guest)).ConvertAll(user => user.User));
        public Synced<List<User>> Arrived
            => new(async () => (await AllUsers.Value()).FindAll(user => user.State.Equals(EventBond.Arrived)).ConvertAll(user => user.User));
        public Synced<List<User>> Left
            => new(async () => (await AllUsers.Value()).FindAll(user => user.State.Equals(EventBond.Left)).ConvertAll(user => user.User));
        public Synced<List<User>> Kicked
            => new(async () => (await AllUsers.Value()).FindAll(user => user.State.Equals(EventBond.Kicked)).ConvertAll(user => user.User));

        public Synced<List<(DateTimeOffset Joined, DateTimeOffset? Left, User User)>> GuestHistory
            => new(() => Terminal.EventDirector.RequestGuestHistoryAsync(this));

        public Synced<List<EventReport>> EventReports
            => new(() => Terminal.DisciplineDirector.RequestEventReportsAsync(this));

        public Synced<List<Etching>> Etchings
            => new(() => Terminal.EtchingDirector.RequestEventEtchingsAsync(this));

		#endregion

		#region Initialisation & Extraction

		public Event() { }

        public Event(EventShard fromEvent)
        {
            Id = fromEvent.Id;
            Host = new(fromEvent.Host);
            Name = fromEvent.Name;
            Description = fromEvent.Description;
            StartTime = fromEvent.StartTime;
            Location = new()
                { Latitude = fromEvent.Latitude, Longitude = fromEvent.Longitude };
            EndTime = fromEvent.TimeEnded;
            State = fromEvent.State;
            GroupMinimum = fromEvent.GroupMinimum;
            GroupMaximum = fromEvent.GroupMaximum;
            Character = new(fromEvent.Character);
            Radius = new() { Kilometres = fromEvent.Radius };
            IsDynamic = fromEvent.IsDynamic;
        }

        public Event(EventThinSlice fromEvent)
        {
            Id = fromEvent.Id;
            Host = new(fromEvent.Host);
            Location = new()
                { Latitude = fromEvent.Latitude, Longitude = fromEvent.Longitude };
        }

        public EventShard ToEventShard()
        {
            return new(Id, Host.ToUserSilhouette(), Name, Description,
                StartTime, Location.Latitude, Location.Longitude, EndTime,
                State, GroupMinimum, GroupMaximum, Character.ToCharacter(),
                Radius.Kilometres, IsDynamic);
        }

        public EventThinSlice ToEventThinSlice()
        {
            return new(Id, Host.ToUserSilhouette(), Location.Latitude, Location.Longitude);
        }

        public EventHeader ToEventHeader(DateTimeOffset lastActiveTime)
        {
            return new(Id, Name, IsActive, lastActiveTime);
        }

		#endregion

		#region Composition

		public bool ValidateAndNormalise()
        { 
            // Sanitise User content
            Name = ContentValidation.NormaliseText(Name[..MaximumNameLength]);
            Description = ContentValidation.NormaliseText(Description[..MaximumDescLength]);

            // Verify Event is within a reasonable time
            if (After(StartTime, Time + OneWeek)) { return false; }

            // Verify group bounds
            if (GroupMaximum != 0 &&
                (GroupMaximum <= GroupMinimum ||
                GroupMaximum < 4)) { return false; }

            return true;
        }

        public async Task<List<(User User, EventBond State)>> GetFriendsOf(User user)
        {
            List<(User User, EventBond State)> friends = new();

            foreach (var userDetails in await AllUsers)
            {
                if (await user.IsFriendsWith(userDetails.User))
                {
                    friends.Add(userDetails);
                }
            }

            return friends;
        }

		#endregion

		#region Checks

		public async Task<bool> IsVisibleTo(User user)
		{
			// Note: This is efficient with multiple users. For multiple events, see User.CanView

			// Check if user account is locked
			if (user.IsLocked)
            { return false; }

			// Check if user's account is limited
			if (!user.CanAttend)
			{
				// User cannot join normal events
                // Check if user can join friend events and Host is friends with the user
				if (!user.CanAttendFriends || !await Host.IsFriendsWith(user))
				{ return false; }
			}

			// Check if user is blocked by or blocking event host
			if (await Host.IsBlockedBy(user) || await Host.IsBlocking(user))
			{ return false; }

			return true;
		}

        public async Task<bool> IsJoinableBy(User user)
        {
            // Check if event is joinable
            if (IsOpen)
            { return false; }

            // Check if user can see event
            if (!await IsVisibleTo(user))
            { return false; }

            // Check if user is kicked from event
            if ((await Kicked).Contains(user))
            { return false; }

            // Check if user or user's haunt is within a reasonable distance
            if (!GeoLocation.AreInRange(await user.LastKnownLocation, Location, MaximumJoinDistance) &&
                !GeoLocation.AreInRange(await user.Haunt, Location, MaximumJoinDistance))
            { return false; }

            return true;
        }

        public bool IsModifiableBy(User user)
        {
			// Check if user is event host
			if (Host.Id.Equals(user.Id))
			{ return true; }

			return false;
        }

        public bool IsHostedBy(User user)
        {
			// Check if user is event host
			if (Host.Id.Equals(user.Id))
			{ return true; }

			return false;
        }

        public async Task<bool> HasUserRelationship(User user)
        {
            // Check if user has interacted with event
            return (await AllUsers).FindAll(x => x.User.Id == user.Id).Count == 1;
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
            // Check if event has not yet started
            if (!IsWaiting)
            { return false; }

            // Check if host is within range
            if (!await IsInRange(Host))
            { return false; }

            return true;
        }

		#endregion

		#region Effects

        public async Task Started()
        {
            _ = NotifyActive($"{Name}", "Event is active!");
        }

        public async Task<List<User>> Ended()
        {
            List<User> updatedGuests = new();

            // Update all participants' vectors and notify
			foreach ((var joined, var left, var guest) in await GuestHistory)
			{
				guest.CalculateCharacter(this, left.Value - joined);

                updatedGuests.Add(guest);

				// Notify of event ending
				_ = guest.Notify($"{Name}", $"Event has concluded.");
			}

            return updatedGuests;
		}

        public async Task Etched(User user)
        {
            // Verify user can etch into the event
            Try(await WasAttendedBy(user),
                new InvalidEventException("User did not attend event."));

            // Verify etching is not before event starting or user is host
            Try(HasAlready(StartTime) || IsModifiableBy(user),
                new InvalidEventException("Event has yet to start."));

            // Verify etching is added before event is closed
            Try(IsActive,
                new InvalidEventException("Event has already ended."));
		}

		public async Task<bool> Reported()
        {
            // Check if there are enough reports
            if ((await EventReports).Count < 3)
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
			return obj is Event other && Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion
	}
}
