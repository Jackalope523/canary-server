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
			var @event = await environment.GenerateUpcomingEventAsync(host, guest);
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
			var @event = await environment.GenerateUpcomingEventAsync(host, guest);
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
			var @event = await environment.GenerateUpcomingEventAsync(host, guest);
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
			Assert.Equal(0, serverCoolEtching.Ratings.Positive);
			Assert.Equal(1, serverCoolEtching.Ratings.Negative);
		}

		[Fact]
		public async Task GetUserFeedAsync_ReturnsFeed()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(host, friend);

			var @event = await environment.GenerateUpcomingEventAsync(host);
			var someEtching = await environment.GenerateEtchingAsync(@event, host);
			var anotherEtching = await environment.GenerateEtchingAsync(@event, host);

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
			var host1 = await environment.GenerateUniqueUserAsync();
			var host2 = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(host1, host2, friend);

			var event1 = await environment.GenerateUpcomingEventAsync(host1);
			var seenEtching = await environment.GenerateEtchingAsync(event1, host1);

			var event2 = await environment.GenerateUpcomingEventAsync(host2);
			var unseenEtching = await environment.GenerateEtchingAsync(event1, host2);

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
