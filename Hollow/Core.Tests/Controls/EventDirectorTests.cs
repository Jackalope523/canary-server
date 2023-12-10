using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
    public class EventDirectorTests : IAsyncLifetime
    {
        private TestEnvironment environment;
		private EventDirector director;

		private User testUser;
		private Event testEvent;

		private DateTimeOffset testStartTime = new(DateTime.UtcNow + TimeSpan.FromDays(1));

        public EventDirectorTests()
        {
            environment = new();
			director = environment.Terminal.EventDirector;
        }

		public async Task InitializeAsync()
		{
			testUser = await environment.GenerateTestUserAsync();
			testEvent = await environment.GenerateTestEventAsync(testUser);
		}

		public Task DisposeAsync()
		{
			environment.Dispose();
			return Task.CompletedTask;
		}

		[Fact]
		public async Task GetEventInformationAsync_ReturnsEvent()
		{
			// Act
			var @event = await director.GetEventInformationAsync(testUser.Id, testEvent.Id);

			// Assert
			Assert.True(testEvent.Equals(@event));
		}

		[Fact]
		public async Task GetEventInformationAsync_BlockedEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var user = await environment.GenerateTestUserAsync();
			await environment.ForceEnemiesAsync(host, user);

			var @event = await environment.GenerateTestEventAsync(host);

			// Act
			var returnedEvent = director.GetEventInformationAsync(user.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await returnedEvent);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_ReturnsEvent()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var user = await environment.GenerateTestUserAsync();

			var @event = await environment.GenerateTestEventAsync(host);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, @event.Location.Latitude, @event.Location.Longitude, 10);

			// Assert
			Assert.Single(nearbyEvents);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_MultipleEvents_ReturnsEvents()
		{
			// Arrange
			var host1 = await environment.GenerateTestUserAsync();
			var host2 = await environment.GenerateTestUserAsync();
			var user = await environment.GenerateTestUserAsync();

			var event1 = await environment.GenerateTestEventAsync(host1);
			var event2 = await environment.GenerateTestEventAsync(host2);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, event1.Location.Latitude, event1.Location.Longitude, 10);

			// Assert
			Assert.Equal(2, nearbyEvents.Count);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_FarEvent_ReturnsCloseEvent()
		{
			// Arrange
			var host1 = await environment.GenerateTestUserAsync();
			var host2 = await environment.GenerateTestUserAsync();
			var user = await environment.GenerateTestUserAsync();

			var event1 = await environment.GenerateTestEventAsync(host1);
			var event2 = environment.CreateTestEvent(host2);
			event2.Location = new() { Latitude = 70, Longitude = 0 };
			await environment.GenerateEventUnsafeAsync(event2, host2);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, event1.Location.Latitude, event1.Location.Longitude, 1);

			// Assert
			Assert.Single(nearbyEvents);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_BlockedEvent_ReturnsNothing()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var user = await environment.GenerateTestUserAsync();
			await environment.ForceEnemiesAsync(host, user);

			var @event = await environment.GenerateTestEventAsync(host);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, @event.Location.Latitude, @event.Location.Longitude, 10);

			// Assert
			Assert.Empty(nearbyEvents);
		}

		[Fact]
		public async Task GetEventsInAreaAsync_NoEvents_ReturnsNothing()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, 90, 0, 1);

			// Assert
			Assert.Empty(nearbyEvents);
		}

		[Fact]
		public async Task GetPersonalisedEventsInAreaAsync_ReturnsEvents()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var user = await environment.GenerateTestUserAsync();

			var @event = await environment.GenerateTestEventAsync(host);

			// Act
			var nearbyEvents = await director.GetEventsInAreaAsync(user.Id, @event.Location.Latitude, @event.Location.Longitude, 10);

			// Assert
			Assert.Single(nearbyEvents);
		}

		[Fact]
		public async Task CreateEventAsync_ValidEvent_ReturnsEvent()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var eventStub = environment.CreateTestEvent(host);
			eventStub.StartTime = testStartTime;

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
			var host = await environment.GenerateTestUserAsync();
			var eventStub = environment.CreateTestEvent(host);
			eventStub.StartTime = testStartTime + TimeSpan.FromDays(30);
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
			var host = await environment.GenerateTestUserAsync();
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
			var host = await environment.GenerateTestUserAsync();
			var @event = environment.GenerateTestEventAsync(host);
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
			var host = await environment.GenerateTestUserAsync();
			var eventStub = await environment.GenerateTestEventAsync(host);
			
			string newDescription = "new description";
			DateTimeOffset newStartTime = testStartTime;
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
			var returnedEvent = await director.GetEventInformationAsync(testUser.Id, testEvent.Id);

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
			Assert.True(false);
		}

		[Fact]
		public async Task EndEventAsync_EndsEvent()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task WatchEventAsync_ValidEvent_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task WatchEventAsync_InvalidEvent_Fails()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task UnwatchEventAsync_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task JoinEventAsync_ValidEvent_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task JoinEventAsync_InvalidEvent_Fails()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task LeaveEventAsync_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task GetGuestListAsync_GuestAtValidEvent_ReturnsGuests()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task GetGuestListAsync_ViewingValidEvent_ReturnsFriends()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task GetGuestListAsync_InvalidEvent_Fails()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task GetPotentialInviteesAsync_ValidEvent_ReturnsUsers()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task InviteUserAsync_ValidFriendValidEvent_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task InviteUserAsync_ValidFriendInvalidEvent_Fails()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task InviteUserAsync_InvalidFriendValidEvent_Fails()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task KickUserAsync_HostedEvent_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task KickUserAsync_NotHostingEvent_Fails()
		{
			Assert.True(false);
		}

	}
}
