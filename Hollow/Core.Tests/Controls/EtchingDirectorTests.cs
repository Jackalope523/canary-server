using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
    public class EtchingDirectorTests : IAsyncLifetime
    {
        private TestEnvironment environment;
		private EtchingDirector director;

		private User testUser;
		private Event testEvent;

        public EtchingDirectorTests()
        {
            environment = new();
			director = environment.Terminal.EtchingDirector;
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
		public async Task GetEventEtchingsAsync_HostedEvent_ReturnsEtchings()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var @event = await environment.GenerateTestEventAsync(host);
			await environment.GenerateTestEtchingAsync(@event, host);
			await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			var serverEtchings = await director.GetEventEtchingsAsync(host.Id, @event.Id);

			// Assert
			Assert.Equal(2, serverEtchings.Count);
		}

		[Fact]
		public async Task GetEventEtchingsAsync_GuestAtEvent_ReturnsEtchings()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var guest = await environment.GenerateTestUserAsync();
			var @event = await environment.GeneratePopulatedEventAsync(host, guest);
			await environment.GenerateTestEtchingAsync(@event, host);
			await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			var serverEtchings = await director.GetEventEtchingsAsync(guest.Id, @event.Id);

			// Assert
			Assert.Equal(2, serverEtchings.Count);
		}

		[Fact]
		public async Task GetEventEtchingsAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var sneakyUser = await environment.GenerateTestUserAsync();
			var @event = await environment.GenerateTestEventAsync(host);
			await environment.GenerateTestEtchingAsync(@event, host);
			await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			var serverEtchings = director.GetEventEtchingsAsync(sneakyUser.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await serverEtchings);
		}

		[Fact]
		public async Task AddEtchingAsync_GuestAtEvent_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var guest = await environment.GenerateTestUserAsync();
			var @event = await environment.GeneratePopulatedEventAsync(host, guest);
			string etchingImageURL = "https://cdn.sparrow.com/0";

			// Act
			await director.AddEtchingAsync(guest.Id, @event.Id, etchingImageURL);

			// Assert
			var serverEtchings = await director.GetEventEtchingsAsync(guest.Id, @event.Id);
			Assert.Single(serverEtchings);
			Assert.Equal(etchingImageURL, serverEtchings[0].ImageURL);
		}

		[Fact]
		public async Task AddEtchingAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var sneakyUser = await environment.GenerateTestUserAsync();
			var @event = await environment.GenerateTestEventAsync(host);
			string etchingImageURL = "https://cdn.sparrow.com/0";

			// Act
			var addEtchingSync = director.AddEtchingAsync(sneakyUser.Id, @event.Id, etchingImageURL);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await addEtchingSync);
		}

		[Fact]
		public async Task RemoveEtchingAsync_OwnedEtching_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var @event = await environment.GenerateTestEventAsync(host);
			var etching = await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			await director.RemoveEtchingAsync(host.Id, etching.Id);

			// Assert
			var serverEtchings = await director.GetEventEtchingsAsync(host.Id, @event.Id);
			Assert.Empty(serverEtchings);
		}

		[Fact]
		public async Task RemoveEtchingAsync_UnownedEtching_Fails()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var sneakyUser = await environment.GenerateTestUserAsync();
			var @event = await environment.GenerateTestEventAsync(host);
			var etching = await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			var removeEtchingSync = director.RemoveEtchingAsync(sneakyUser.Id, etching.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await removeEtchingSync);
		}

		[Fact]
		public async Task RateEtchingAsync_ValidEtching_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var guest = await environment.GenerateTestUserAsync();
			var @event = await environment.GeneratePopulatedEventAsync(host, guest);
			var coolEtching = await environment.GenerateTestEtchingAsync(@event, host);
			var uglyEtching = await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			await director.RateEtchingAsync(guest.Id, coolEtching.Id, UserRating.Positive);
			await director.RateEtchingAsync(guest.Id, uglyEtching.Id, UserRating.Negative);

			// Assert
			var serverEtchings = await director.GetEventEtchingsAsync(host.Id, @event.Id);
			
			var serverCoolEtching = serverEtchings.Find(etching => etching.Id.Equals(coolEtching.Id));
			Assert.Equal(1, serverCoolEtching.Ratings.Positive);
			Assert.Equal(0, serverCoolEtching.Ratings.Negative);

			var serverUglyEtching = serverEtchings.Find(etching => etching.Id.Equals(uglyEtching.Id));
			Assert.Equal(0, serverCoolEtching.Ratings.Positive);
			Assert.Equal(1, serverCoolEtching.Ratings.Negative);
		}

		[Fact]
		public async Task GetUserFeedAsync_ReturnsFeed()
		{
			// Arrange
			var host = await environment.GenerateTestUserAsync();
			var friend = await environment.GenerateTestUserAsync();
			await environment.ForceFriendshipAsync(host, friend);

			var @event = await environment.GenerateTestEventAsync(host);
			var someEtching = await environment.GenerateTestEtchingAsync(@event, host);
			var anotherEtching = await environment.GenerateTestEtchingAsync(@event, host);

			// Act
			var (feedDepth, feedHeaders, feedEtchings) = await director.GetUserFeedAsync(friend.Id, int.MaxValue);

			// Assert
			Assert.Single(feedHeaders);
			Assert.Equal(@event.Id, feedHeaders[0].Id);

			Assert.Equal(2, feedEtchings.Count);
			var serverSomeEtching = feedEtchings.Find(etching => etching.Id.Equals(someEtching.Id));
			Assert.Equal(@event.Id, serverSomeEtching.EventId);
			Assert.Equal(someEtching.Id, serverSomeEtching.Id);
		}

		[Fact]
		public async Task GetUserFeedAsync_ExcludingEvent_ReturnsFeed()
		{
			// Arrange
			var host1 = await environment.GenerateTestUserAsync();
			var host2 = await environment.GenerateTestUserAsync();
			var friend = await environment.GenerateTestUserAsync();
			await environment.ForceFriendshipAsync(host1, host2, friend);

			var event1 = await environment.GenerateTestEventAsync(host1);
			var seenEtching = await environment.GenerateTestEtchingAsync(event1, host1);

			var event2 = await environment.GenerateTestEventAsync(host2);
			var unseenEtching = await environment.GenerateTestEtchingAsync(event1, host2);

			// Act
			var (feedDepth, feedHeaders, feedEtchings) = await director.GetUserFeedAsync(friend.Id, int.MaxValue, new() { event1.Id });

			// Assert
			Assert.Single(feedHeaders);
			Assert.Equal(event2.Id, feedHeaders[0].Id);

			Assert.Single(feedEtchings);
			var serverSomeEtching = feedEtchings.Find(etching => etching.Id.Equals(unseenEtching.Id));
			Assert.Equal(event2.Id, serverSomeEtching.EventId);
			Assert.Equal(unseenEtching.Id, serverSomeEtching.Id);
		}
	}
}
