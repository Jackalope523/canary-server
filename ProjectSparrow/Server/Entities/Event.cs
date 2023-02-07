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
        public string EventType { get; set; }
        public DateTimeOffset StartTime { get; init; }
        public GeoLocation Location { get; set; }
        public DateTimeOffset? EndTime { get; init; }
        public bool IsOpen { get; set; }
        public int GroupMinimum { get; set; }
        public int GroupMaximum { get; set; }

        public List<ThinnerUser> Attendees { get; set; }

        public IList<Participant> Participants => ImmutableList.CreateRange(participantLog.ActiveParticipants.ToList());

        private readonly ParticipantLog participantLog;

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
            EventType = fromEvent.EventType;
            StartTime = fromEvent.StartTime;
            Location = new()
                { Latitude = fromEvent.Latitude, Longitude = fromEvent.Longitude };
            EndTime = fromEvent.TimeEnded;
            IsOpen = fromEvent.IsOpen;
            GroupMinimum = fromEvent.GroupMinimum;
            GroupMaximum = fromEvent.GroupMaximum;
        }

        public Event(ThinnerEvent fromEvent)
        {
            Id = fromEvent.Id;
            Host = new(fromEvent.Host);
            EventType = fromEvent.EventType;
            Location = new()
                { Latitude = fromEvent.Latitude, Longitude = fromEvent.Longitude };
        }

        public ThinEvent ToThinEvent()
        {
            return new(Id, Host.ToThinnerUser(), Name, Description, EventType,
                StartTime, Location.Latitude, Location.Longitude, EndTime,
                IsOpen, GroupMinimum, GroupMaximum);
        }

        public ThinnerEvent ToThinnerEvent()
        {
            return new(Id, Host.ToThinnerUser(), EventType, Location.Latitude, Location.Longitude);
        }

        public async Task<bool> IsVisibleTo(Guid userID)
        {
			// Check if user is blocked by event host
			if (await Host.IsBlocking(userID))
			{ return false; }

			return true;
		}

        public async Task<bool> ModifiableBy(Guid userID)
        {
			// Check if user is event host
			if (Host.Id == userID)
			{ return true; }

			return false;
        }

        public async Task<bool> AttendedBy(Guid userID)
        {
            Attendees ??= await EventManager.Manager.GetAttendeesInternalAsync(Id);

            // Check if user is on the guest list
            return Attendees.Find(x => x.Id == userID) != null;
		}
    }
}
