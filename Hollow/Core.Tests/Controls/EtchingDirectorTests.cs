using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Controls
{
    public class EtchingDirectorTests : CoreTest
    {
		private EtchingDirector director;

        public EtchingDirectorTests()
        {
			director = environment.Terminal.EtchingDirector;
        }

		[Fact]
		public async Task GetEventEtchingsAsync_HostedEvent_ReturnsEtchings()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.GenerateEtchingAsync(@event, host);
			await environment.GenerateEtchingAsync(@event, host);

			// Act
			var serverEtchings = await director.GetEventEtchingsAsync(host.Id, @event.Id);

			// Assert
			Assert.Equal(2, serverEtchings.Count);
		}

		[Fact]
		public async Task GetEventEtchingsAsync_GuestAtEvent_ReturnsEtchings()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateOngoingEventAsync(host, guest);
			await environment.GenerateEtchingAsync(@event, host);
			await environment.GenerateEtchingAsync(@event, host);

			// Act
			var serverEtchings = await director.GetEventEtchingsAsync(guest.Id, @event.Id);

			// Assert
			Assert.Equal(2, serverEtchings.Count);
		}

		[Fact]
		public async Task GetEventEtchingsAsync_InvalidEvent_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			await environment.GenerateEtchingAsync(@event, host);
			await environment.GenerateEtchingAsync(@event, host);

			// Act
			var serverEtchings = director.GetEventEtchingsAsync(sneakyUser.Id, @event.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await serverEtchings);
		}

		[Fact]
		public async Task AddEtchingAsync_GuestAtEvent_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateOngoingEventAsync(host, guest);
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
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
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
			var host = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			var etching = await environment.GenerateEtchingAsync(@event, host);

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
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateUpcomingEventAsync(host);
			var etching = await environment.GenerateEtchingAsync(@event, host);

			// Act
			var removeEtchingSync = director.RemoveEtchingAsync(sneakyUser.Id, etching.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await removeEtchingSync);
		}

		[Fact]
		public async Task RateEtchingAsync_ValidEtching_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var @event = await environment.GenerateOngoingEventAsync(host, guest);
			var coolEtching = await environment.GenerateEtchingAsync(@event, host);
			var uglyEtching = await environment.GenerateEtchingAsync(@event, host);

			// Act
			await director.RateEtchingAsync(guest.Id, coolEtching.Id, UserRating.Positive);
			await director.RateEtchingAsync(guest.Id, uglyEtching.Id, UserRating.Negative);

			// Assert
			var serverEtchings = await director.GetEventEtchingsAsync(host.Id, @event.Id);
			
			var serverCoolEtching = serverEtchings.Find(etching => etching.Id.Equals(coolEtching.Id));
			Assert.Equal(1, serverCoolEtching.Ratings.Positive);
			Assert.Equal(0, serverCoolEtching.Ratings.Negative);

			var serverUglyEtching = serverEtchings.Find(etching => etching.Id.Equals(uglyEtching.Id));
			Assert.Equal(0, serverUglyEtching.Ratings.Positive);
			Assert.Equal(1, serverUglyEtching.Ratings.Negative);
		}

		[Fact]
		public async Task GetUserFeedAsync_ReturnsFeed()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(host, friend);

			var @event = await environment.GeneratePastEventAsync(host);
			var someEtching = await environment.GenerateEtchingAsync(@event, host);
			var anotherEtching = await environment.GenerateEtchingAsync(@event, host);

			// Act
			var feed = await director.GetUserFeedAsync(friend.Id, int.MaxValue, 0);

			// Assert
			Assert.Single(feed.Headers);
			Assert.Equal(@event.Id, feed.Headers[0].Id);

			Assert.Equal(2, feed.Etchings.Count);
			var serverSomeEtching = feed.Etchings.Find(etching => etching.Id.Equals(someEtching.Id));
			Assert.Equal(@event.Id, serverSomeEtching.EventId);
			Assert.Equal(someEtching.Id, serverSomeEtching.Id);
		}

		[Fact]
		public async Task GetUserFeedAsync_ExcludingEvent_ReturnsFeed()
		{
			// Arrange
			var host1 = await environment.GenerateUniqueUserAsync();
			var host2 = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(host1, host2, friend);

			var event1 = await environment.GenerateOngoingEventAsync(host1);
			var seenEtching = await environment.GenerateEtchingAsync(event1, host1);

			var event2 = await environment.GeneratePastEventAsync(host2);
			var unseenEtching = await environment.GenerateEtchingAsync(event1, host2);

			// Act
			var feed = await director.GetUserFeedAsync(friend.Id, int.MaxValue, 1);

			// Assert
			Assert.Single(feed.Headers);
			Assert.Equal(event2.Id, feed.Headers[0].Id);

			Assert.Single(feed.Etchings);
			var serverSomeEtching = feed.Etchings.Find(etching => etching.Id.Equals(unseenEtching.Id));
			Assert.Equal(event2.Id, serverSomeEtching.EventId);
			Assert.Equal(unseenEtching.Id, serverSomeEtching.Id);
		}
	}
}
