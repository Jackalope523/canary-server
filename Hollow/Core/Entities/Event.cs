using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Shared;
using Core.Boundaries;
using Core.Controls;

namespace Core.Entities
{
    internal class Event
    {
        #region Variables

        public const int MaximumNameLength = 50;
        public const int MaximumDescLength = 400;

        public readonly Distance MaximumJoinDistance = new() { Kilometres = 200 };

        public ulong Id { get; init; }
        public User Host { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CharacterVector Character { get; set; }
        public DateTimeOffset StartTime { get; init; }
        public GeoLocation Location { get; set; }
        public DateTimeOffset? EndTime { get; init; }
        public bool IsOpen { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }

        public List<UserSilhouette> Guests { get; set; }
        public List<(DateTimeOffset Joined, DateTimeOffset? Left, UserSilhouette User)> AllGuests { get; set; }

		public List<EventReport> EventReports { get; set; }

        public List<Etching> EventEtchings { get; set; }

		#endregion

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
                IsOpen, GroupMinimum, GroupMaximum, Character.ToCharacter());
        }

        public EventThinSlice ToEventThinSlice()
        {
            return new(Id, Host.ToUserSilhouette(), Location.Latitude, Location.Longitude);
        }

        public EventHeader ToEventHeader(DateTimeOffset lastActiveTime)
        {
            return new(Id, Name, EndTime.HasValue, lastActiveTime);
        }

		public async Task SyncReports()
		{
			EventReports = await CoreTerminal.Terminal.ReportDirector.GetEventReportsAsync(Id);
		}

        public async Task SyncEtchings()
        {
            EventEtchings = await CoreTerminal.Terminal.EtchingDirector.GetEventEtchingsAsync(Id);
        }

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

        public async Task<bool> IsModifiableBy(ulong userId)
            => await IsModifiableBy(new User(userId));

        public async Task<bool> IsModifiableBy(User user)
        {
			// Check if user is event host
			if (Host.Id == user.Id)
			{ return true; }

			return false;
        }

        public async Task<bool> IsAttendedBy(ulong userId)
            => await IsAttendedBy(new User(userId));

        public async Task<bool> IsAttendedBy(User user)
        {
            Guests ??= await CoreTerminal.Terminal.EventDirector.GetGuestsInternalAsync(Id);

            // Check if user is on the guest list
            return Guests.Find(x => x.Id == user.Id) != null;
		}

        public async Task<bool> WasAttendedBy(ulong userId)
            => await WasAttendedBy(new User(userId));

        public async Task<bool> WasAttendedBy(User user)
        {
            AllGuests ??= await CoreTerminal.Terminal.EventDirector.GetAllGuestsInternalAsync(Id);

            // Check if user is or was on the guest list
            return AllGuests.Find(x => x.User.Id == user.Id).User != null;
		}

        public async Task<bool> Reported()
        {
            await SyncReports();

            // Check if there are enough reports
            if (EventReports.Count < 3)
            { return false; }

            return true;
        }
    }
}
