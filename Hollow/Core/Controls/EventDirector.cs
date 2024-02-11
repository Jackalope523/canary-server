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
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);

			// Verify user is allowed to view event
			Try(await targetEvent.IsVisibleTo(user),
				new InvalidEventException("User is unable to view event."));

			return targetEvent.ToEventShard();
		}

		public async Task<List<EventThinSlice>> GetEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUserAsync(userId);
			var nearbyEvents = await Events.FindEventsAsync(latitude, longitude, distance);

			// Remove events from list that the user cannot access
			nearbyEvents = await RemoveInaccessibleEventsAsync(user, nearbyEvents);

			return nearbyEvents;
		}

		public async Task<List<EventThinSlice>> GetPersonalisedEventsInAreaAsync(ulong userId,
			double latitude, double longitude, double distance)
		{
			var user = await GetUserAsync(userId);
			var nearbyEvents = await Events.FindEventsAsync(latitude, longitude, distance);

			// Remove inaccessible events and events with a large difference between event and user interest
			nearbyEvents = await RemoveUnattractiveEventsAsync(user, nearbyEvents, 1f);

			return nearbyEvents;
		}

		public async Task<EventShard> CreateEventAsync(ulong userId,
			string eventName, string eventDescription, DateTimeOffset startTime,
			double latitude, double longitude,
			double radius, bool isDynamic, int? groupMinimum, int? groupMaximum)
		{
			var user = await GetUserAsync(userId);
			// Verify user is already at an event
			await ThrowIfUserAtEvent(user);

			// Verify user can host
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

			// Verify user has no conflict
			var conflict = (await user.UpcomingEvents).Find(e => IsWithin(e.StartTime - eventStub.StartTime, HalfHour));
			if (conflict != null)
			{ throw new InvalidEventException($"User has event {conflict.Id} conflict."); }

			// Try to create an event
			var newEvent = await Events.CreateEventAsync(user.Id, eventStub.Name, eventStub.Description,
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
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);

			// Verify user is event host
			Try(targetEvent.IsModifiableBy(user),
				new InvalidEventException("User is unable to edit event."));

			// Verify event is still active
			Try(targetEvent.IsActive,
				new InvalidEventException("Unable to edit event, event has ended."));

			// Fail if edits may not be done during the event
			Fail(HasAlready(targetEvent.StartTime) &&
				(!string.IsNullOrEmpty(eventDescription) || IsNotNull(startTime) ||
				AreNotNull(latitude, longitude) ||
				IsNotNull(radius) || IsNotNull(isDynamic)),
				new InvalidEventException("Cannot edit certain event attributes once it has started."));

			Event editedEvent = new(targetEvent.ToEventShard())
			{
				Description = eventDescription,
				State = IsNull(isOpen) ? targetEvent.State : (isOpen.Value ? EventState.Open : EventState.Sealed),
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
				edits.Add(("Location", (editedEvent.Location.Latitude, editedEvent.Location.Longitude)));
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
			await Events.UpdateEventAsync(targetEvent.Id, edits);

			_ = targetEvent.NotifyActive($"{targetEvent.Name}", "The event was edited by the host, check to see the updates.");
		}

		public async Task StartEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);
			_ = targetEvent.Host.LastKnownLocation.Sync();

			// Verify user is host
			Try(targetEvent.IsHostedBy(user),
				new InvalidUserException("User is not the host of this event"));

			// Verify event can be started
			Try(await targetEvent.IsStartable(),
				new InvalidEventException("Event cannot be started."));

			// Try to start event
			await Events.UpdateEventAsync(targetEvent.Id, new() { (nameof(EventShard.State), EventState.Open) });
			await Events.SetUserStateAsync(user.Id, targetEvent.Id, EventBond.Arrived);

			await targetEvent.Started();
		}

		public async Task EndEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);

			// Verify user is able to end the event
			Try(targetEvent.IsModifiableBy(user),
				new InvalidUserException("User does not have permissions to end event."));

			// Try to end to event
			await Events.EndEventAsync(eventId);

			var participants = await targetEvent.Ended();

			// Update all participants' vectors
			_ = Terminal.AccountDirector.UpdateAllAsync(participants, user => new() { (nameof(UserShard.Character), user.Character) });
		}

		public async Task WatchEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);

			// Verify user is allowed to view event
			Try(await targetEvent.IsVisibleTo(user),
				new InvalidEventException($"User is unable to watch event.\nAccount Status: {user.AccountStatus}"));

			Fail(targetEvent.EndTime.HasValue,
				new InvalidEventException("User is unable to watch event, event has ended."));

			EventBond? userIntention = null;

			try
			{
				userIntention = await Events.GetUserStateAsync(userId, eventId);
			}
			catch { }

			// Ensure correct state transition
			if (!userIntention.HasValue)
			{
				// Try to add user to the event
				await Events.SetUserStateAsync(userId, eventId, EventBond.Watching);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not watch event, user currently {userIntention.Value} event."); }
		}

		public async Task UnwatchEventAsync(ulong userId, ulong eventId)
		{
            EventBond? userIntention = null;

            try
            {
                userIntention = await Events.GetUserStateAsync(userId, eventId);
            }
            catch { }

			// Ensure correct state transition
			if (userIntention.HasValue &&
				userIntention.Value.Equals(EventBond.Watching))
			{
				// Try to remove user from event
				await Events.RemoveUserAsync(userId, eventId);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not unwatch event, user currently {userIntention.Value} event."); }
		}

		public async Task JoinEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);
			_ = user.LastKnownLocation.Sync();

			// Verify user is allowed to join event
			Try(await targetEvent.IsJoinableBy(user),
				new InvalidEventException($"User is unable to join event.\nAccount Status: {user.AccountStatus}"));

			// Check if user has an active event conflict
			if (HasAlready(targetEvent.StartTime))
			{ await ThrowIfUserAtEvent(user); }
			else
			{
				// Check if user has an upcoming conflict
				var conflict = (await user.UpcomingEvents).Find(e => IsWithin(e.StartTime - targetEvent.StartTime, HalfHour));
				if (conflict != null)
				{ throw new InvalidEventException($"User has event {conflict.Id} conflict."); }
			}
			// Check if event is active and user is already there
			if (HasAlready(targetEvent.StartTime) &&
				await targetEvent.IsInRange(user))
			{
				// Try to add user to the event
				await Events.SetUserStateAsync(user.Id, targetEvent.Id, EventBond.Arrived);
			}
			else
			{
				// Try to add user to the event
				await Events.SetUserStateAsync(userId, eventId, EventBond.Guest);
			}			

			// Notify host if event has already started
			if (HasAlready(targetEvent.StartTime))
			{ _ = targetEvent.Host.Notify($"Sparrower Inbound", $"{user.Name} is joining your event."); }
		}

		public async Task LeaveEventAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);

			// Verify user is the host
			Fail(targetEvent.IsHostedBy(user),
				new InvalidUserException("Host cannot leave the event."));

            // Get the user's current status
            var userIntention = await Events.GetUserStateAsync(userId, eventId);
			
			// Check if user is guest or arrived
			if (userIntention.Equals(EventBond.Arrived))
			{
				// Try to remove user from event
				await Events.SetUserStateAsync(user.Id, targetEvent.Id, EventBond.Left);
			}
			else if (userIntention.Equals(EventBond.Guest))
			{
				// Try to remove user from event
				await Events.RemoveUserAsync(user.Id, targetEvent.Id);
			}
			else if (userIntention.HasValue)
			{ throw new InvalidOperationException($"Could not leave event, user currently {userIntention.Value} event."); }
		}

		public async Task<(int Watchers, int GuestCount, List<(UserSilhouette User, EventBond State)> Guests)>
			GetGuestListAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var targetEvent = await GetEventAsync(eventId);

			(int Watchers, int GuestCount, List<(UserSilhouette User, EventBond State)> Guests)
				guestList = new(0, 0, new());

			// Check if user is host
			if (targetEvent.IsModifiableBy(user))
			{
				// Retrieve user's friends that are watching
				var friends = await targetEvent.GetFriendsOf(user);

				guestList.Guests.AddRange(SelectAsSilhouette(friends,
					friend => friend.State.Equals(EventBond.Watching)));

				// Add visible users
				guestList.Guests.AddRange(SelectAsSilhouette(await targetEvent.AllUsers,
					user => !user.State.Equals(EventBond.Watching)));

				guestList.GuestCount = targetEvent.IsOngoing ? (await targetEvent.Arrived).Count : (await targetEvent.Left).Count;
				guestList.Watchers = (await targetEvent.Watching).Count;
			}
			// Check if user is a guest
			else if (await targetEvent.WasAttendedBy(user))
			{
				// Retrieve user's friends watching or attending
				var friends = await targetEvent.GetFriendsOf(user);

				guestList.Guests.AddRange(SelectAsSilhouette(friends,
					friend => friend.State.Equals(EventBond.Watching) || friend.State.Equals(EventBond.Guest)));

				// Add visible users
				guestList.Guests.AddRange(SelectAsSilhouette(await targetEvent.AllUsers,
					user => user.State.Equals(EventBond.Arrived) || user.State.Equals(EventBond.Left)));

				guestList.GuestCount = targetEvent.IsOngoing ? (await targetEvent.Arrived).Count : (await targetEvent.Left).Count;
				guestList.Watchers = guestList.Guests.Where(guest => guest.State.Equals(EventBond.Watching)).Count();
            }
			// Check if user can view event
			else if (await targetEvent.IsVisibleTo(user))
			{
				// Retrieve user's friends that will be, are, or were attending
				var friends = await targetEvent.GetFriendsOf(user);
				guestList.Guests = SelectAsSilhouette(friends, _ => true);

				// Add visible information
				guestList.GuestCount = targetEvent.IsOngoing ? (await targetEvent.Arrived).Count : (await targetEvent.Left).Count;
				guestList.Watchers = SelectAsSilhouette(friends, friend => friend.State.Equals(EventBond.Watching)).Count;
			}
			// User cannot recieve information about event
			else
			{ throw new InvalidUserException("User cannot view event."); }

			return guestList;
		}

		public async Task<List<UserSilhouette>> GetPotentialInviteesAsync(ulong userId, ulong eventId)
		{
			var user = await GetUserAsync(userId);
			var @event = await GetEventAsync(eventId);

			List<User> potentialUsers = new();

			// Add all friends that can join event
			foreach (var friend in await user.Friends)
			{
				if (await @event.IsJoinableBy(friend))
				{ potentialUsers.Add(friend); }
			}

			return potentialUsers
				.ConvertAll(u => u.ToUserSilhouette());
		}

		public async Task InviteUserAsync(ulong inviterId, ulong inviteeId, ulong eventId)
		{
			var inviter = await GetUserAsync(inviterId);
			var invitee = await GetUserAsync(inviteeId);
			var @event = await GetEventAsync(eventId);

			// Verify inviter has relationship with event
			Try(await @event.HasUserRelationship(inviter),
				new InvalidEventException("User must be watching, a guest, or arrived at event to invite."));

			// Verify that the invitee can join the event
			Try(await @event.IsJoinableBy(invitee),
				new InvalidUserException("Invited cannot join event."));

			// Verify that inviter is friends with the invitee
			Try(await inviter.IsFriendsWith(invitee),
				new InvalidUserException("Cannot invite non-friends."));

			_ = invitee.PostNote(inviter, $"has invited you to {@event.Name}", $"{@event.Id}");
			_ = invitee.Notify("Sparrow", "You were invited to ");
		}

		public async Task KickUserAsync(ulong hostId, ulong targetId, ulong eventId)
		{
			var host = await GetUserAsync(hostId);
			var targetUser = await GetUserAsync(targetId);
			var @event = await GetEventAsync(eventId);

			// Verify kicking user is the host
			Try(@event.IsHostedBy(host),
				new InvalidUserException("User cannot kick guests."));

			// Verify event is active
			Try(@event.IsActive,
				new InvalidEventException("Cannot kick users after event has been archived."));

			// Verify host is not kicking themself
			Fail(host.Equals(targetUser),
				new InvalidUserException("Host cannot kick themself."));

			// Kick target user from event
			await Events.SetUserStateAsync(targetUser.Id, @event.Id, EventBond.Kicked);

			// Hide target user's etchings from event
			foreach (Etching etching in await @event.Etchings)
			{
				if (targetUser.Etched(etching))
				{ _ = Etchings.HideEtchingAsync(etching.Id); }
			}
		}

		#endregion

		#region Favours

		internal async Task<Event> RequestCurrentEventForUserAsync(User user)
		{
			var currentEvent = await Events.FindCurrentEventForUserAsync(user.Id);

			return currentEvent != null ? new(currentEvent) : Event.None;
		}

		internal async Task<List<Event>> RequestPastEventsForUserAsync(User user)
		{
			return (await Events.FindPastEventsForUserAsync(user.Id))
				.ConvertAll(@event => new Event(@event));
		}

		internal async Task<List<Event>> RequestUpcomingEventsForUserAsync(User user)
		{
			return (await Events.FindUpcomingEventsForUserAsync(user.Id))
				.ConvertAll(@event => new Event(@event));
		}
		
		internal async Task<List<(User User, EventBond State)>> RequestAllUsersFromEventAsync(Event @event)
		{
			return (await Events.GetAllUsersAsync(@event.Id))
				.ConvertAll(userDetails => (new User(userDetails.User), userDetails.State));
		}

		internal async Task<List<(DateTimeOffset Joined, DateTimeOffset? Left, User User)>>
			RequestGuestHistoryAsync(Event @event)
		{
			return (await Events.GetGuestHistoryAsync(@event.Id))
				.ConvertAll(userDetails => (userDetails.Joined, userDetails.Left, new User(userDetails.User)));
		}

		internal async Task<List<EventShard>>
			RemoveInaccessibleEventsAsync(User user, List<EventShard> events)
		{
			foreach (EventShard e in events)
			{
				Event targetEvent = new(e);

				if (!await user.CanView(targetEvent))
				{ events.Remove(e); }
			}

			return events;
		}

		internal async Task<List<EventThinSlice>>
			RemoveInaccessibleEventsAsync(User user, List<EventThinSlice> events)
		{
			List<EventThinSlice> inaccessibleEvents = new();

			foreach (EventThinSlice e in events)
			{
				Event targetEvent = new(e);

				if (!await user.CanView(targetEvent))
				{ inaccessibleEvents.Add(e); }
			}

			events = events.Except(inaccessibleEvents).ToList();

			return events;
		}

		internal async Task<List<EventThinSlice>>
			RemoveUnattractiveEventsAsync(User user, List<EventThinSlice> events, float maximumAngle)
        {
            List<EventThinSlice> inaccessibleEvents = new();

            foreach (EventThinSlice e in events)
			{
				Event targetEvent = new(e);

				if (!await user.CanJoin(targetEvent))
				{ inaccessibleEvents.Remove(e); continue; }

				if (CharacterVector.AngleBetweenAffected(user.Character, targetEvent.Character) > maximumAngle)
				{ inaccessibleEvents.Remove(e); }
            }

            events = events.Except(inaccessibleEvents).ToList();

            return events;
		}

		#endregion

		#region Tools

		private async Task ThrowIfUserAtEvent(User user)
		{
			Fail(await user.IsAtEvent(),
				new InvalidUserException("User is currently attending an event."));
		}

		private List<(UserSilhouette User, EventBond State)>
			SelectAsSilhouette(List<(User User, EventBond State)> users, Func<(User User, EventBond State), bool> predicate)
		{
			return users.Where(predicate).ToList().ConvertAll(userDetails => (userDetails.User.ToUserSilhouette(), userDetails.State));
		}

		#endregion
	}
}
