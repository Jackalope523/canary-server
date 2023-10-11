using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Shared;
using Server.Boundaries;
using Server.Controls;

namespace Server.Entities
{
    internal class Event
    {
        #region Variables

        public Guid Id { get; init; }
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

        public List<ThinnerUser> Attendees { get; set; }

		public List<EventReport> EventReports { get; set; }

        public List<EventPost> EventPosts { get; set; }

		#endregion

		public Event() { }

        public Event(Guid eventID)
        {
            Id = eventID;
        }

        public Event(ThinEvent fromEvent)
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

        public Event(ThinnerEvent fromEvent)
        {
            Id = fromEvent.Id;
            Host = new(fromEvent.Host);
            Location = new()
                { Latitude = fromEvent.Latitude, Longitude = fromEvent.Longitude };
        }

        public ThinEvent ToThinEvent()
        {
            return new(Id, Host.ToThinnerUser(), Name, Description,
                StartTime, Location.Latitude, Location.Longitude, EndTime,
                IsOpen, GroupMinimum, GroupMaximum, Character.ToCharacter());
        }

        public ThinnerEvent ToThinnerEvent()
        {
            return new(Id, Host.ToThinnerUser(), Location.Latitude, Location.Longitude);
        }

        public EventHeader ToEventHeader(DateTimeOffset lastActiveTime)
        {
            return new(Id, Name, EndTime.HasValue, lastActiveTime);
        }

		public async Task SyncReports()
		{
			EventReports = await EventManager.Manager.GetEventReportsAsync(Id);
		}

        public async Task SyncPosts()
        {
            EventPosts = await EventManager.Manager.GetEventPostsAsync(Id);
        }

        public bool ValidateAndNormalise()
        {
            // Sanitise User content
            Name = ContentValidation.NormaliseText(Name);
            Description = ContentValidation.NormaliseText(Description);

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

			// Check if user is blocked by event host
			if (await Host.IsBlocking(user))
			{ return false; }

			return true;
		}

        public async Task<bool> IsModifiableBy(Guid userID)
            => await IsModifiableBy(new User(userID));

        public async Task<bool> IsModifiableBy(User user)
        {
			// Check if user is event host
			if (Host.Id == user.Id)
			{ return true; }

			return false;
        }

        public async Task<bool> IsAttendedBy(Guid userID)
            => await IsAttendedBy(new User(userID));

        public async Task<bool> IsAttendedBy(User user)
        {
            Attendees ??= await EventManager.Manager.GetAttendeesInternalAsync(Id);

            // Check if user is on the guest list
            return Attendees.Find(x => x.Id == user.Id) != null;
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
