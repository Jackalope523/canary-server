using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Shared;
using Core.Boundaries;
using Core.Controls;
using Microsoft.Extensions.Logging;

namespace Core.Entities
{
    internal class Event
    {
        #region Variables

        public const int MaximumNameLength = 50;
        public const int MaximumDescLength = 400;

        public readonly Distance MaximumJoinDistance = new() { Kilometres = 200 };
        public readonly Distance GuestDistance = new() { Metres = 75 };

        public ulong Id { get; init; }
        public User Host { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CharacterVector Character { get; set; }
        public DateTimeOffset StartTime { get; init; }
        public GeoLocation Location { get; set; }
        public Distance Radius { get; set; }
        public bool IsDynamic { get; set; }
        public DateTimeOffset? EndTime { get; init; }
        public bool IsOpen { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }

        public List<(User User, EventUserState State)> AllUsers { get; set; }
        public List<User> Watchers { get; set; }
        public List<User> Incoming { get; set; }
        public List<User> Guests { get; set; }
        public List<User> Left { get; set; }
        public List<(DateTimeOffset Joined, DateTimeOffset? Left, User User)> GuestHistory { get; set; }

		public List<EventReport> EventReports { get; set; }

        public List<Etching> Etchings { get; set; }

		#endregion

		#region Initialisation & Extraction

		public Event() { }

        public Event(ulong eventId)
        {
            Id = eventId;
        }

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
            IsOpen = fromEvent.IsOpen;
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
                IsOpen, GroupMinimum, GroupMaximum, Character.ToCharacter(),
                Radius.Kilometres, IsDynamic);
        }

        public EventThinSlice ToEventThinSlice()
        {
            return new(Id, Host.ToUserSilhouette(), Location.Latitude, Location.Longitude);
        }

        public EventHeader ToEventHeader(DateTimeOffset lastActiveTime)
        {
            return new(Id, Name, EndTime.HasValue, lastActiveTime);
        }

		#endregion

		#region Synchronisation

		public async Task SyncReports()
		{
			EventReports = await CoreTerminal.Terminal.ReportDirector.RequestEventReportsAsync(this);
		}

        public async Task SyncUsers()
        {
            AllUsers = await CoreTerminal.Terminal.EventDirector.RequestAllUsersFromEventAsync(this);
            Watchers = AllUsers.FindAll(user => user.State.Equals(EventUserState.Watching)).ConvertAll(user => user.User);
            Incoming = AllUsers.FindAll(user => user.State.Equals(EventUserState.Attending)).ConvertAll(user => user.User);
            Guests = AllUsers.FindAll(user => user.State.Equals(EventUserState.Present)).ConvertAll(user => user.User);
            Left = AllUsers.FindAll(user => user.State.Equals(EventUserState.Left)).ConvertAll(user => user.User);
        }

        public async Task SyncEtchings()
        {
            Etchings = await CoreTerminal.Terminal.EtchingDirector.RequestEventEtchingsAsync(this);
        }

		#endregion

		#region Composition

		public bool ValidateAndNormalise()
        { 
            // Sanitise User content
            Name = ContentValidation.NormaliseText(Name[..MaximumNameLength]);
            Description = ContentValidation.NormaliseText(Description[..MaximumDescLength]);

            // Verify Event is within a reasonable time
            if (StartTime > DateTimeOffset.UtcNow + TimeSpan.FromDays(7)) { return false; }

            // Verify group bounds
            if (GroupMaximum != 0 &&
                (GroupMaximum <= GroupMinimum ||
                GroupMaximum < 4)) { return false; }

            return true;
        }

        public async Task<List<(User User, EventUserState State)>> GetFriendsOf(User user)
        {
            List<(User User, EventUserState State)> friends = new();

            foreach (var userDetails in AllUsers)
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
			// Note: This is efficient when user is singular but highly
            // inefficient if being called multiple times with different users.
			// If the second case, create a User method to accomplish this method (multiple events, same user)
			// and change this one to be efficient for the second case (same event, multiple users).
            // Currently, no use case warrants one event checking multiple users.

			// Check if user account is locked
			if (user.IsLocked)
            { return false; }

			// Check if user's account is limited
			if (!user.CanAttend)
			{
				// User cannot join normal events
                // Check if user can join friend events and Host is friends with the user
				if (!user.CanAttendFriends || !await user.IsFriendsWith(Host))
				{ return false; }
			}

			// Check if user is blocked by or blocking event host
			if (await user.IsBlockedBy(Host) || await user.IsBlocking(Host))
			{ return false; }

            // Check if user or user's haunt is within a reasonable distance
            await user.SyncLocation();
            if (!GeoLocation.AreInRange(user.LastKnownLocation, Location, MaximumJoinDistance) &&
                !GeoLocation.AreInRange(user.Haunt, Location, MaximumJoinDistance))
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

        public async Task<bool> IsAttendedBy(User user)
        {
            Guests ??= await CoreTerminal.Terminal.EventDirector.RequestGuestsAsync(this);

            // Check if user is on the guest list
            return Guests.Find(x => x.Id == user.Id) != null;
		}

        public async Task<bool> WasAttendedBy(User user)
        {
            if (Guests == null || Left == null)
            { await SyncUsers(); }

            // Check if user is or was on the guest list
            return Guests.Find(x => x.Id == user.Id) != null || Left.Find(x => x.Id == user.Id) != null;
		}

        public bool IsInRange(User user)
            => GeoLocation.AreInRange(Location, user.LastKnownLocation, GuestDistance);

		#endregion

		#region Effects

        public async Task<List<User>> Ended()
        {
            await SyncUsers();

            List<User> updatedGuests = new();

            // Update all participants' vectors and notify
			foreach ((var joined, var left, var guest) in GuestHistory)
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
			// Ensure the user can etch into the event
			if (!await WasAttendedBy(user))
			{ throw new InvalidEventException("User did not attend event."); }

			// Ensure etching is added within a day of event ending
			if (EndTime.HasValue && EndTime + TimeSpan.FromDays(1) < DateTimeOffset.UtcNow)
			{ throw new InvalidEventException("Event has already ended."); }
		}

		public async Task<bool> Reported()
        {
            await SyncReports();

            // Check if there are enough reports
            if (EventReports.Count < 3)
            { return false; }

            return true;
        }

        #endregion

        #region Actions

        public async Task NotifyGuests(string title, string message)
        {
            if (Guests == null)
            { await SyncUsers(); }

            foreach (var guest in Guests)
            {
                if (IsHostedBy(guest))
                { continue; }

                _ = guest.Notify(title, message);
            }
        }

        #endregion
    }
}
