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
			Assert.True(false); // Big TODO
		}
	}
}
