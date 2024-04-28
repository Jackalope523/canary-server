using Core.Boundaries;
using Core.Controls;
using Core.Entities;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Controls
{
    public class EventDirectorTests : CoreTest
    {
		private EventDirector director;

        public EventDirectorTests()
        {
			director = environment.Terminal.EventDirector;
        }

		[Fact]
		public async Task GetEventInformationAsync_Host_ReturnsEvent()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			Event returnedEvent = new(await director.GetEventInformationAsync(host.Id, @event.Id));

			// Assert
			Assert.True(@event.Equals(returnedEvent));
		}

		[Fact]
		public async Task GetEventInformationAsync_Neutral_ReturnsEvent()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			Event returnedEvent = new(await director.GetEventInformationAsync(user.Id, @event.Id));

			// Assert
			Assert.True(@event.Equals(returnedEvent));
		}

		[Fact]
		public async Task GetEventInformationAsync_BlockedEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(host, user);

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var returnedEvent = director.GetEventInformationAsync(user.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await returnedEvent);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_ReturnsEvent()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUniqueEventAsync(host);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, @event.Location.Latitude, @event.Location.Longitude, 10);

			// Assert
			Assert.Single(nearbyEvents);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_MultipleEvents_ReturnsEvents()
		{
			// Arrange
			var host1 = await environment.GenerateUniqueUserAsync();
			var host2 = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var events = await environment.GenerateMultipleUniqueEventAsync(host1, host2);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, events[0].Location.Latitude, events[0].Location.Longitude, 10);

			// Assert
			Assert.Equal(2, nearbyEvents.Count);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_FarEvent_ReturnsCloseEvent()
		{
			// Arrange
			var host1 = await environment.GenerateUniqueUserAsync();
			var host2 = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var closeEvent = await environment.GenerateUniqueEventAsync(host1);
			await environment.GenerateUniqueEventAsync(host2);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, closeEvent.Location.Latitude, closeEvent.Location.Longitude, 1);

			// Assert
			Assert.Single(nearbyEvents);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_BlockedEvent_ReturnsNothing()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(host, user);

			var @event = await environment.GenerateUniqueEventAsync(host);
			
			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, @event.Location.Latitude, @event.Location.Longitude, 10);

			// Assert
			Assert.Empty(nearbyEvents);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_NoEvents_ReturnsNothing()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, 90, 0, 1);

			// Assert
			Assert.Empty(nearbyEvents);
		}

		[Fact]
		public async Task GetPersonalisedEventsInAreaAsync_ReturnsEvents()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, @event.Location.Latitude, @event.Location.Longitude, 10);

			// Assert
			Assert.Single(nearbyEvents);
		}

		[Fact]
		public async Task CreateEventAsync_ValidEvent_ReturnsEvent()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var eventStub = environment.CreateTestEvent(host);
			eventStub.StartTime = new(DateTime.UtcNow + TimeSpan.FromDays(1));

			// Act
			var returnedEvent = await director.CreateEventAsync(host.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.Radius.Kilometres, eventStub.IsDynamic,
				eventStub.GroupMinimum, eventStub.GroupMaximum);

			// Assert
			Assert.Equal(eventStub.Name, returnedEvent.Name);
			Assert.Equal(eventStub.Description, returnedEvent.Description);
			Assert.Equal(eventStub.StartTime, returnedEvent.StartTime);

			Assert.Equal(eventStub.Location.Latitude, returnedEvent.Latitude);
			Assert.Equal(eventStub.Location.Longitude, returnedEvent.Longitude);
			Assert.Equal(eventStub.Radius.Kilometres, returnedEvent.Radius);
			Assert.Equal(eventStub.IsDynamic, returnedEvent.IsDynamic);

			Assert.Equal(eventStub.GroupMinimum, returnedEvent.GroupMinimum);
			Assert.Equal(eventStub.GroupMaximum, returnedEvent.GroupMaximum);
		}

		[Fact]
		public async Task CreateEventAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var eventStub = environment.CreateTestEvent(host);
			eventStub.StartTime = new(DateTime.UtcNow + TimeSpan.FromDays(30));
			eventStub.GroupMaximum = 3;
			eventStub.GroupMinimum = 5;

			// Act
			var returnedEvent = director.CreateEventAsync(host.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.Radius.Kilometres, eventStub.IsDynamic,
				eventStub.GroupMinimum, eventStub.GroupMaximum);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await returnedEvent);
		}

		[Fact]
		public async Task CreateEventAsync_UserCannotHost_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			await environment.UpdateUser(host, nameof(UserShard.AccountStatus), UserAccountStatus.Impotent);

			var eventStub = environment.CreateTestEvent(host);

			// Act
			var returnedEvent = director.CreateEventAsync(host.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.Radius.Kilometres, eventStub.IsDynamic,
				eventStub.GroupMinimum, eventStub.GroupMaximum);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await returnedEvent);
		}

		[Fact]
		public async Task CreateEventAsync_EventConflict_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = environment.GenerateUpcomingEventAsync(host);
			var conflictingEvent = environment.CreateTestEvent(host);

			// Act
			var returnedEvent = director.CreateEventAsync(host.Id, conflictingEvent.Name, conflictingEvent.Description,
				conflictingEvent.StartTime, conflictingEvent.Location.Latitude, conflictingEvent.Location.Longitude,
				conflictingEvent.Radius.Kilometres, conflictingEvent.IsDynamic,
				conflictingEvent.GroupMinimum, conflictingEvent.GroupMaximum);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await returnedEvent);
		}

		[Fact]
		public async Task EditEventAsync_ValidInput_UpdatesEvent()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var eventStub = await environment.GenerateUpcomingEventAsync(host);
			
			string newDescription = "new description";
			DateTimeOffset newStartTime = new(DateTime.UtcNow + TimeSpan.FromDays(2));
			bool newIsOpen = false;

			double newLatitude = 50, newLongitude = 50, newRadius = 3.14;
			bool newIsDynamic = true;

			int newGroupMinimum = 7, newGroupMaximum = 41;

			// Act
			await director.EditEventAsync(host.Id, eventStub.Id, eventDescription: newDescription,
				isOpen: newIsOpen, startTime: newStartTime,
				latitude: newLatitude, longitude: newLongitude,
				radius: newRadius, isDynamic: newIsDynamic,
				groupMinimum: newGroupMinimum, groupMaximum: newGroupMaximum);

			// Assert
			var returnedEvent = await director.GetEventInformationAsync(host.Id, eventStub.Id);

			Assert.Equal(newDescription, returnedEvent.Description);
			Assert.Equal(newStartTime, returnedEvent.StartTime);
			Assert.Equal(newIsOpen, new Event(returnedEvent).IsOpen);

			Assert.Equal(newLatitude, returnedEvent.Latitude);
			Assert.Equal(newLongitude, returnedEvent.Longitude);
			Assert.Equal(newRadius, returnedEvent.Radius);
			Assert.Equal(newIsDynamic, returnedEvent.IsDynamic);

			Assert.Equal(newGroupMinimum, returnedEvent.GroupMinimum);
			Assert.Equal(newGroupMaximum, returnedEvent.GroupMaximum);
		}

		[Fact]
		public async Task StartEventAsync_StartsEvent()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			var @event = environment.CreateTestEvent(user);
			@event.StartTime = DateTimeOffset.UtcNow;

			@event = await environment.GenerateEventUnsafeAsync(@event, user);
			await environment.UpdateUserLocationAsync(user, @event.Location.Latitude, @event.Location.Longitude);

			// Act
			await director.StartEventAsync(user.Id, @event.Id);

			// Assert
			Event startedEvent = new(await environment.Terminal.EventDatabase.FindEventAsync(@event.Id));
			Assert.Equal(@event, startedEvent);
			Assert.Equal(EventState.Open, startedEvent.State);
		}

		[Fact]
		public async Task EndEventAsync_EndsEvent()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateOngoingEventAsync(user);

			// Act
			await director.EndEventAsync(user.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task WatchEventAsync_ValidEvent_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			await director.WatchEventAsync(user.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task WatchEventAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GeneratePastEventAsync(host);

			// Act
			var watchSync = director.WatchEventAsync(user.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await watchSync);
		}

		[Fact]
		public async Task UnwatchEventAsync_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
			await director.WatchEventAsync(user.Id, @event.Id);

			// Act
			await director.UnwatchEventAsync(user.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task JoinEventAsync_ValidEvent_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
            await environment.UpdateUserLocationAsync(user, @event.Location.Latitude, @event.Location.Longitude);

            // Act
            await director.JoinEventAsync(user.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task JoinEventAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GeneratePastEventAsync(host);
            await environment.UpdateUserLocationAsync(user, @event.Location.Latitude, @event.Location.Longitude);

            // Act
            var join = director.WatchEventAsync(user.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await join);
		}

		[Fact]
		public async Task LeaveEventAsync_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(host);
            await environment.UpdateUserLocationAsync(user, @event.Location.Latitude, @event.Location.Longitude);
            await director.JoinEventAsync(user.Id, @event.Id);

			// Act
			await director.LeaveEventAsync(user.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task GetGuestListAsync_HostingEvent_ReturnsUsers()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var left = await environment.GenerateUniqueUserAsync();
			var incoming = await environment.GenerateUniqueUserAsync();
			var watcher = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateOngoingEventAsync(user, guest);
			await environment.AddUserToEventAsync(@event, left, EventBond.Left);
			await environment.AddUserToEventAsync(@event, incoming, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, watcher, EventBond.Watching);

			// Act
			var (Watchers, GuestCount, Guests) = await director.GetGuestListAsync(user.Id, @event.Id);

			// Assert
			Assert.Equal(1, Watchers);
			Assert.Equal(2, GuestCount);

			Assert.Equal(2, Guests.Where(user => user.State.Equals(EventBond.Arrived)).Count());
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Left)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Guest)));
			Assert.Empty(Guests.Where(user => user.State.Equals(EventBond.Watching)));
		}

		[Fact]
		public async Task GetGuestListAsync_GuestAtValidEvent_ReturnsGuests()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var left = await environment.GenerateUniqueUserAsync();
			var incoming = await environment.GenerateUniqueUserAsync();
			var incomingFriend = await environment.GenerateUniqueUserAsync();
			var watcher = await environment.GenerateUniqueUserAsync();
			var watchingFriend = await environment.GenerateUniqueUserAsync();

			await environment.ForceFriendshipAsync(user, incomingFriend, watchingFriend);

			var @event = await environment.GenerateOngoingEventAsync(host, user);
			await environment.AddUserToEventAsync(@event, left, EventBond.Left);
			await environment.AddUserToEventAsync(@event, incoming, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, incomingFriend, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, watcher, EventBond.Watching);
			await environment.AddUserToEventAsync(@event, watchingFriend, EventBond.Watching);

			// Act
			var (Watchers, GuestCount, Guests) = await director.GetGuestListAsync(user.Id, @event.Id);

			// Assert
			Assert.Equal(1, Watchers);
			Assert.Equal(2, GuestCount);

			Assert.Equal(2, Guests.Where(user => user.State.Equals(EventBond.Arrived)).Count());
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Left)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Guest)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Watching)));
		}

		[Fact]
		public async Task GetGuestListAsync_ViewingValidEvent_ReturnsFriends()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var guestFriend = await environment.GenerateUniqueUserAsync();
			var left = await environment.GenerateUniqueUserAsync();
			var leftFriend = await environment.GenerateUniqueUserAsync();
			var incoming = await environment.GenerateUniqueUserAsync();
			var incomingFriend = await environment.GenerateUniqueUserAsync();
			var watcher = await environment.GenerateUniqueUserAsync();
			var watchingFriend = await environment.GenerateUniqueUserAsync();

			await environment.ForceFriendshipAsync(user, guestFriend, leftFriend, incomingFriend, watchingFriend);

			var @event = await environment.GenerateOngoingEventAsync(host, guestFriend);
			await environment.AddUserToEventAsync(@event, left, EventBond.Left);
			await environment.AddUserToEventAsync(@event, leftFriend, EventBond.Left);
			await environment.AddUserToEventAsync(@event, incoming, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, incomingFriend, EventBond.Guest);
			await environment.AddUserToEventAsync(@event, watcher, EventBond.Watching);
			await environment.AddUserToEventAsync(@event, watchingFriend, EventBond.Watching);

			// Act
			var (Watchers, GuestCount, Guests) = await director.GetGuestListAsync(user.Id, @event.Id);

			// Assert
			Assert.Equal(1, Watchers);
			Assert.Equal(2, GuestCount);

			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Arrived)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Left)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Guest)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Watching)));
		}

		[Fact]
		public async Task GetGuestListAsync_ViewingPastEvent_ReturnsFriends()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			var stranger = await environment.GenerateUniqueUserAsync();

			await environment.ForceFriendshipAsync(user, friend);

			var @event = await environment.GeneratePastEventAsync(host, friend, stranger);

			// Act
			var (Watchers, GuestCount, Guests) = await director.GetGuestListAsync(user.Id, @event.Id);

			// Assert
			Assert.Equal(0, Watchers);
			Assert.Equal(3, GuestCount);

			Assert.Empty(Guests.Where(user => user.State.Equals(EventBond.Arrived)));
			Assert.Single(Guests.Where(user => user.State.Equals(EventBond.Left)));
			Assert.Empty(Guests.Where(user => user.State.Equals(EventBond.Guest)));
			Assert.Empty(Guests.Where(user => user.State.Equals(EventBond.Watching)));
		}

		[Fact]
		public async Task GetGuestListAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			await environment.ForceEnemiesAsync(user, host);

			var @event = await environment.GenerateOngoingEventAsync(host);

			// Act
			var guestList = director.GetGuestListAsync(user.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await guestList);
		}

		[Fact]
		public async Task GetPotentialInviteesAsync_ValidEvent_ReturnsUsers()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var @event = await environment.GenerateUpcomingEventAsync(host, user);
			await environment.UpdateUserLocationAsync(friend, @event.Location.Latitude, @event.Location.Longitude);

			// Act
			var invitees = await director.GetPotentialInviteesAsync(user.Id, @event.Id);

			// Assert
			Assert.Single(invitees);
		}

		[Fact]
		public async Task InviteUserAsync_ValidFriendValidEvent_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var @event = await environment.GenerateUpcomingEventAsync(host, user);
            await environment.UpdateUserLocationAsync(friend, @event.Location.Latitude, @event.Location.Longitude);

            // Act
            await director.InviteUserAsync(user.Id, friend.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task InviteUserAsync_ValidFriendInvalidEvent_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var host = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(user, friend);

			var @event = await environment.GenerateUpcomingEventAsync(host);

			// Act
			var invite = director.InviteUserAsync(user.Id, friend.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await invite);
		}

		[Fact]
		public async Task InviteUserAsync_InvalidFriendValidEvent_Fails()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var stranger = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateUpcomingEventAsync(user);

			// Act
			var invite = director.InviteUserAsync(user.Id, stranger.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await invite);
		}

		[Fact]
		public async Task KickUserAsync_HostedEvent_Succeeds()
		{
			// Arrange
			var user = await environment.GenerateUniqueUserAsync();
			var stranger = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateOngoingEventAsync(user, stranger);

			// Act
			await director.KickUserAsync(user.Id, stranger.Id, @event.Id);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task KickUserAsync_NotHostingEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var user = await environment.GenerateUniqueUserAsync();
			var stranger = await environment.GenerateUniqueUserAsync();

			var @event = await environment.GenerateOngoingEventAsync(host, user, stranger);

			// Act
			var kick = director.KickUserAsync(user.Id, stranger.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await kick);
		}

	}
}
