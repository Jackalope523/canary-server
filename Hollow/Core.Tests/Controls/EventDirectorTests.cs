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
		public async Task GetEventInformationAsync_ValidId_ReturnsEvent()
		{
			// Act
			var @event = await director.GetEventInformationAsync(testUser.Id, testEvent.Id);

			// Assert
			Assert.True(testEvent.Equals(@event));
		}

		[Fact]
		public async Task GetEventsInAreaAsync_ReturnsEvents()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task GetPersonalisedEventsInAreaAsync_ReturnsEvents()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task CreateEventAsync_ValidEvent_Succeeds()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task CreateEventAsync_InvalidEvent_Fails()
		{
			Assert.True(false);
		}

		[Fact]
		public async Task EditEventAsync_ValidInput_UpdatesEvent()
		{
			Assert.True(false);
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
