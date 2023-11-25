using Core.Boundaries;
using Core.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using static Core.Entities.Arbiter;
using static Core.Entities.Artificer;
using static Core.Entities.Psijic;

namespace Core.Controls
{
	internal class EventDirector : AbstractDirector, IEventOperations
	{
		#region Initialisation

		public EventDirector(CoreTerminal terminal) : base(terminal) { }

		#endregion

		#region Operations

		public async Task<EventShard> GetEventInformationAsync(ulong userId, ulong eventId)
        {
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is allowed to view event
			Try(await targetEvent.IsVisibleTo(user),
				new InvalidEventException("User is unable to view event."));

			return targetEvent.ToEventShard();
		}

		public async Task<List<EventThinSlice>> GetEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUser(userId);
			var nearbyEvents = Events.FindEvents(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			await RemoveInaccessibleEventsAsync(user, nearbyEvents);

			return nearbyEvents;
		}

		public async Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUser(userId);
			var nearbyEvents = Events.FindEvents(latitude, longitude, distance);

			// Remove inaccessible events and events with a large difference between event and user interest
			await RemoveUnattractiveEventsAsync(user, nearbyEvents, 1f);

			return nearbyEvents;
		}

		public async Task<EventShard> CreateEventAsync(ulong userId,
			string eventName, string eventDescription, DateTimeOffset startTime,
			double latitude, double longitude,
			double radius, bool isDynamic, int? groupMinimum, int? groupMaximum)
		{
			var user = await GetUser(userId);
			// Check if user is already at an event
			await ThrowIfUserAtEvent(user);

			// Check if user can host
			Try(user.CanHost,
				new InvalidUserException("User cannot host.\n" +
				$"Account Status: {user.AccountStatus}"));

			// Create event
			Event eventStub = new()
			{
				Name = eventName,
				Description = eventDescription,
				StartTime = startTime,
				Location = new() { Latitude = latitude, Longitude = longitude },
				GroupMinimum = groupMinimum ?? 0,
				GroupMaximum = groupMaximum ?? 0,
				Radius = new() { Kilometres = Math.Clamp(radius, 0.1, radius) },
				IsDynamic = isDynamic,
			};

			// Validate event
			Try(eventStub.ValidateAndNormalise(),
				new InvalidInformationException("Invalid event details provided."));

			// Verify that user has no conflict
			await user.SyncUpcomingEvents();
			var conflict = user.UpcomingEvents.Find(e => IsWithin(e.StartTime - eventStub.StartTime, HalfHour));
			if (conflict != null)
			{ throw new InvalidEventException($"User has event {conflict.Id} conflict."); }

			// Try to create an event
			var newEvent = Events.CreateEvent(user.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.GroupMinimum, eventStub.GroupMaximum, user.Character.ToCharacter(),
				eventStub.Radius.Kilometres, eventStub.IsDynamic);

			// Notify followers of event
			_ = user.NotifyFollowers($"New Sparrow Event", $"{user.Name} just created a new event {newEvent.Name}");

			return newEvent;
		}

		public async Task EditEventAsync(ulong userId, ulong eventId,
			string eventDescription = "", bool? isOpen = null,
			DateTimeOffset? startTime = null, double? latitude = null, double? longitude = null,
			double? radius = null, bool? isDynamic = null, int? groupMinimum = null, int? groupMaximum = null)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Verify that user is event host
			Try(targetEvent.IsModifiableBy(user),
				new InvalidEventException("User is unable to edit event."));

			// Verify that event is still active
			Try(targetEvent.IsActive,
				new InvalidEventException("Unable to edit event, event has ended."));

			// Check for edits that may not be done during the event
			Fail(HasAlready(targetEvent.StartTime) &&
				(!string.IsNullOrEmpty(eventDescription) || IsNotNull(startTime) ||
				AreNotNull(latitude, longitude) ||
				IsNotNull(radius) || IsNotNull(isDynamic)),
				new InvalidEventException("Cannot edit certain event attributes once it has started."));

			Event editedEvent = new(targetEvent.ToEventShard())
			{
				Description = eventDescription,
				State = IsNull(isOpen) ? targetEvent.State : (isOpen.Value ? EventState.active_open : EventState.active_closed),
				StartTime = startTime ?? targetEvent.StartTime,
				Location = AreNull(latitude, longitude) ? targetEvent.Location : new() { Latitude = latitude.Value, Longitude = longitude.Value },
				Radius = IsNull(radius) ? targetEvent.Radius : new() { Kilometres = Math.Clamp(radius.Value, 0.1, radius.Value) },
				IsDynamic = isDynamic ?? targetEvent.IsDynamic,
				GroupMinimum = groupMinimum ?? targetEvent.GroupMinimum,
				GroupMaximum = groupMaximum ?? targetEvent.GroupMaximum,
			};

			// Validate event
			Try(editedEvent.ValidateAndNormalise(),
				new InvalidInformationException("Invalid event details provided."));

			List<(string Property, object Value)> edits = new();

			// Gather individual edits
			if (!string.IsNullOrEmpty(eventDescription))
			{
				edits.Add((nameof(EventShard.Description), editedEvent.Description));
			}
			if (IsNotNull(isOpen))
			{
				edits.Add((nameof(EventShard.State), editedEvent.State));
			}
			if (IsNotNull(startTime))
			{
				edits.Add((nameof(EventShard.StartTime), editedEvent.StartTime));
			}
			if (IsNotNull(latitude) && IsNotNull(longitude))
			{
				edits.Add((nameof(EventShard.Latitude), editedEvent.Location.Latitude));
				edits.Add((nameof(EventShard.Longitude), editedEvent.Location.Longitude));
			}
			if (IsNotNull(radius))
			{
				edits.Add((nameof(EventShard.Radius), editedEvent.Radius));
			}
			if (IsNotNull(isDynamic))
			{
				edits.Add((nameof(EventShard.IsDynamic), editedEvent.IsDynamic));
			}
			if (IsNotNull(groupMinimum))
			{
				edits.Add((nameof(EventShard.GroupMinimum), editedEvent.GroupMinimum));
			}
			if (IsNotNull(groupMaximum))
			{
				edits.Add((nameof(EventShard.GroupMaximum), editedEvent.GroupMaximum));
			}

			// Push update
			Try(Events.UpdateEvent(targetEvent.Id, edits),
				new UnexpectedFailureException("Failed to edit event."));

			_ = targetEvent.NotifyActive($"{targetEvent.Name}", "This event was edited by the host, check to see the updates.");
		}

		public async Task StartEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);
			await targetEvent.Host.SyncLocation();

			// Check if the user is host
			Try(targetEvent.IsHostedBy(user),
				new InvalidUserException("User is not the host of this event"));

			// Check if the event can be started
			Try(targetEvent.IsStartable(),
				new InvalidEventException("Event cannot be started."));

			// Try to start event
			Try(Events.UpdateEvent(targetEvent.Id, new() { (nameof(EventShard.State), EventState.active_open) }),
				new UnexpectedFailureException("Could not start event."));

			await targetEvent.Started();
		}

		public async Task EndEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if the user is able to end the event
			Try(targetEvent.IsModifiableBy(user),
				new InvalidUserException("User does not have permissions to end event."));

			// Try to end to event
			Try(Events.EndEvent(eventId),
				new UnexpectedFailureException("Could not end event."));

			var participants = await targetEvent.Ended();

			// Update all participants' vectors
			_ = Terminal.AccountDirector.UpdateAllAsync(participants, user => new() { (nameof(UserShard.Character), user.Character) });
		}

		public async Task WatchEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is allowed to view event
			Try(await targetEvent.IsVisibleTo(user),
				new InvalidEventException($"User is unable to view or join event.\nAccount Status: {user.AccountStatus}"));

			var userIntention = Events.GetUserState(userId, eventId);

			// Ensure correct state transition
			if (!userIntention.HasValue)
			{
				// Try to add user to the event
				Try(Events.SetUserState(userId, eventId, EventUserState.Watching),
					new UnexpectedFailureException("Could not watch event."));
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not watch event, user currently {userIntention.Value} event."); }
		}

		public async Task UnwatchEventAsync(ulong userId, ulong eventId)
		{
			var userIntention = Events.GetUserState(userId, eventId);

			// Ensure correct state transition
			if (userIntention.HasValue &&
				userIntention.Value.Equals(EventUserState.Watching))
			{
				// Try to remove user from event
				Try(Events.RemoveUser(userId, eventId),
					new UnexpectedFailureException("Could not unwatch event."));
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not unwatch event, user currently {userIntention.Value} event."); }
		}

		public async Task JoinEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is allowed to view event
			Try(await targetEvent.IsVisibleTo(user),
				new InvalidEventException($"User is unable to view or join event.\nAccount Status: {user.AccountStatus}"));

			// Check if event is open
			Try(targetEvent.IsOpen,
				new InvalidEventException("Event is closed."));

			// Check if user has an active event conflict
			if (HasAlready(targetEvent.StartTime))
			{ await ThrowIfUserAtEvent(user); }
			else
			{
				await user.SyncUpcomingEvents();

				// Check if user has an upcoming conflict
				var conflict = user.UpcomingEvents.Find(e => IsWithin(e.StartTime - targetEvent.StartTime, HalfHour));
				if (conflict != null)
				{ throw new InvalidEventException($"User has event {conflict.Id} conflict."); }
			}

			bool success;

			// Check if event is active and user is already there
			if (HasAlready(targetEvent.StartTime) &&
				targetEvent.IsInRange(user))
			{
				// Try to add user to the event
				success = Events.SetUserState(user.Id, targetEvent.Id, EventUserState.Present);
			}
			else
			{
				// Try to add user to the event
				success = Events.SetUserState(userId, eventId, EventUserState.Attending);
			}

			Try(success, new UnexpectedFailureException("Could not join event."));

			// Notify host if event has already started
			if (HasAlready(targetEvent.StartTime))
			{ _ = targetEvent.Host.Notify($"Sparrower Inbound", $"{user.Name} is joining your event."); }
		}

		public async Task LeaveEventAsync(ulong userId, ulong eventId)
		{
			User user = new(userId);
			var targetEvent = await GetEvent(eventId);

			// Check if user is the host
			Try(targetEvent.IsHostedBy(user),
				new InvalidUserException("Host cannot leave the event."));

			// Get the user's current status
			var userIntention = Events.GetUserState(user.Id, targetEvent.Id);

			if (userIntention.Equals(EventUserState.Present))
			{
				// Try to remove user from event
				Try(Events.SetUserState(user.Id, targetEvent.Id, EventUserState.Left),
					new UnexpectedFailureException("Could not leave event."));
			}
			else if (userIntention.Equals(EventUserState.Attending))
			{
				// Try to remove user from event
				Try(Events.RemoveUser(user.Id, targetEvent.Id),
					new UnexpectedFailureException("Could not unattend event."));
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not leave event, user currently {userIntention.Value} event."); }
		}

		public async Task<(int Watchers, int GuestCount, List<(UserSilhouette User, EventUserState State)> Guests)>
			GetGuestListAsync(ulong userId, ulong eventId)
		{
			var user = await GetUser(userId);
			var targetEvent = await GetEvent(eventId);

			(int Watchers, int GuestCount, List<(UserSilhouette User, EventUserState State)> Guests)
				guestList = new(0, 0, new());

			await targetEvent.SyncUsers();

			// Check if user is host
			if (targetEvent.IsModifiableBy(user))
			{
				// Retrieve user's friends that are watching
				var friends = await targetEvent.GetFriendsOf(user);

				guestList.Guests.AddRange(SelectAsSilhouette(friends,
					friend => friend.State.Equals(EventUserState.Watching)));

				// Add visible users
				guestList.Guests.AddRange(SelectAsSilhouette(targetEvent.AllUsers,
					user => !user.State.Equals(EventUserState.Watching)));

				guestList.GuestCount = targetEvent.Guests.Count;
				guestList.Watchers = targetEvent.Watchers.Count;
			}
			// Check if user is a guest
			else if (await targetEvent.WasAttendedBy(user))
			{
				// Retrieve user's friends watching or attending
				var friends = await targetEvent.GetFriendsOf(user);

				guestList.Guests.AddRange(SelectAsSilhouette(friends,
					friend => friend.State.Equals(EventUserState.Watching) || friend.State.Equals(EventUserState.Attending)));

				// Add visible users
				guestList.Guests.AddRange(SelectAsSilhouette(targetEvent.AllUsers,
					user => user.State.Equals(EventUserState.Present) || user.State.Equals(EventUserState.Left)));

				guestList.GuestCount = targetEvent.Guests.Count;
			}
			// Check if user can view event
			else if (await targetEvent.IsVisibleTo(user))
			{
				// Retrieve user's friends that will be, are, or were attending
				var friends = await targetEvent.GetFriendsOf(user);

				guestList.Guests = SelectAsSilhouette(friends, friend => !friend.State.Equals(EventUserState.Watching));

				// Add visible information
				guestList.GuestCount = targetEvent.Guests.Count;
			}
			// User cannot recieve information about event
			else
			{ throw new InvalidUserException("User cannot view event."); }

			return guestList;
		}

		#endregion

		#region Favours

		internal async Task<Event> RequestCurrentEventForUserAsync(User user)
		{
			return new(Events.FindCurrentEventForUser(user.Id));
		}

		internal async Task<List<Event>> RequestPastEventsForUserAsync(User user)
		{
			return Events.FindPastEventsForUser(user.Id)
				.ConvertAll(@event => new Event(@event));
		}

		internal async Task<List<Event>> RequestUpcomingEventsForUserAsync(User user)
		{
			return Events.FindUpcomingEventsForUser(user.Id)
				.ConvertAll(@event => new Event(@event));
		}
		
		internal async Task<List<(User User, EventUserState State)>> RequestAllUsersFromEventAsync(Event @event)
		{
			return Events.GetAllUsers(@event.Id)
				.ConvertAll(userDetails => (new User(userDetails.User), userDetails.State));
		}

		internal async Task<List<User>> RequestGuestsAsync(Event @event)
		{
			return Events.GetGuests(@event.Id)
				.ConvertAll(user => new User(user));
		}

		internal async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, User User)>>
			RequestAllGuestsAsync(Event @event)
		{
			return Events.GetGuestHistory(@event.Id)
				.ConvertAll(userDetails => (userDetails.Joined, userDetails.Left, new User(userDetails.User)));
		}

		internal async Task<List<EventShard>>
			RemoveInaccessibleEventsAsync(User user, List<EventShard> events)
		{
			foreach (EventShard e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(user))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<List<EventThinSlice>>
			RemoveInaccessibleEventsAsync(User user, List<EventThinSlice> events)
		{
			foreach (EventThinSlice e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsVisibleTo(user))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<List<EventThinSlice>>
			RemoveUnattractiveEventsAsync(User user, List<EventThinSlice> events, float maximumAngle)
		{
			foreach (EventThinSlice e in events)
			{
				Event targetEvent = new(e);

				if (!await targetEvent.IsJoinableBy(user))
				{ events.Remove(e); continue; }

				if (CharacterVector.AngleBetweenAffected(user.Character, targetEvent.Character) > maximumAngle)
				{ events.Remove(e); }
			}

			return events;
		}

		#endregion

		#region Tools

		private async Task ThrowIfUserAtEvent(User user)
		{
			Fail(await user.IsAtEvent(),
				new InvalidUserException("User is currently attending an event."));
		}

		private List<(UserSilhouette User, EventUserState State)>
			SelectAsSilhouette(List<(User User, EventUserState State)> users, Func<(User User, EventUserState State), bool> predicate)
		{
			return users.Where(predicate).ToList().ConvertAll(userDetails => (userDetails.User.ToUserSilhouette(), userDetails.State));
		}

		#endregion
	}
}
