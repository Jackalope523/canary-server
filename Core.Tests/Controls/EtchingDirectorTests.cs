using Core.Boundaries;
using Core.Controls;
using Core.Entities;

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
		public async Task GetGatheringEtchingsAsync_HostedGathering_ReturnsEtchings()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateUpcomingGatheringAsync(host);
			await environment.GenerateEtchingAsync(@gathering, host);
			await environment.GenerateEtchingAsync(@gathering, host);

			// Act
			var serverEtchings = await director.GetGatheringEtchingsAsync(host.Id, @gathering.Id);

			// Assert
			Assert.Equal(2, serverEtchings.Count);
		}

		[Fact]
		public async Task GetGatheringEtchingsAsync_GuestAtGathering_ReturnsEtchings()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateOngoingGatheringAsync(host, guest);
			await environment.GenerateEtchingAsync(@gathering, host);
			await environment.GenerateEtchingAsync(@gathering, host);

			// Act
			var serverEtchings = await director.GetGatheringEtchingsAsync(guest.Id, @gathering.Id);

			// Assert
			Assert.Equal(2, serverEtchings.Count);
		}

		[Fact]
		public async Task GetGatheringEtchingsAsync_InvalidGathering_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateUpcomingGatheringAsync(host);
			await environment.GenerateEtchingAsync(@gathering, host);
			await environment.GenerateEtchingAsync(@gathering, host);

			// Act
			var serverEtchings = director.GetGatheringEtchingsAsync(sneakyUser.Id, @gathering.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await serverEtchings);
		}

		[Fact]
		public async Task AddEtchingAsync_GuestAtGathering_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateOngoingGatheringAsync(host, guest);
			byte[] image = { byte.MinValue, 0, 1, 3, byte.MaxValue, 7, 8 };

			// Act
			await director.AddEtchingAsync(guest.Id, @gathering.Id, new(image));

			// Assert
			var serverEtchings = await director.GetGatheringEtchingsAsync(guest.Id, @gathering.Id);
			Assert.Single(serverEtchings);

			var etching = serverEtchings[0];
			Assert.Equal(image, (await environment.Terminal.MediaDatabase.DownloadImageAsync(etching.Id, guest.Id)).ToArray());
		}

		[Fact]
		public async Task AddEtchingAsync_InvalidGathering_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateUpcomingGatheringAsync(host);

			// Act
			var addEtchingSync = director.AddEtchingAsync(sneakyUser.Id, @gathering.Id, new(0));

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await addEtchingSync);
		}

		[Fact]
		public async Task RemoveEtchingAsync_OwnedEtching_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateUpcomingGatheringAsync(host);
			var etching = await environment.GenerateEtchingAsync(@gathering, host);

			// Act
			await director.RemoveEtchingAsync(host.Id, etching.Id);

			// Assert
			var serverEtchings = await director.GetGatheringEtchingsAsync(host.Id, @gathering.Id);
			Assert.Empty(serverEtchings);
		}

		[Fact]
		public async Task RemoveEtchingAsync_UnownedEtching_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var @gathering = await environment.GenerateUpcomingGatheringAsync(host);
			var etching = await environment.GenerateEtchingAsync(@gathering, host);

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
			var @gathering = await environment.GenerateOngoingGatheringAsync(host, guest);
			var coolEtching = await environment.GenerateEtchingAsync(@gathering, host);
			var uglyEtching = await environment.GenerateEtchingAsync(@gathering, host);

			// Act
			await director.RateEtchingAsync(guest.Id, coolEtching.Id, UserRating.Positive);
			await director.RateEtchingAsync(guest.Id, uglyEtching.Id, UserRating.Negative);

			// Assert
			var serverEtchings = await director.GetGatheringEtchingsAsync(host.Id, @gathering.Id);
			
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

			var @gathering = await environment.GeneratePastGatheringAsync(host);
			var someEtching = await environment.GenerateEtchingAsync(@gathering, host);
			var anotherEtching = await environment.GenerateEtchingAsync(@gathering, host);

			// Act
			var feed = await director.GetUserFeedAsync(friend.Id, 100, 0);

			// Assert
			Assert.Single(feed.Headers);
			Assert.Equal(@gathering.Id, feed.Headers[0].Id);

			Assert.Equal(2, feed.Etchings.Count);
			var serverSomeEtching = feed.Etchings.Find(etching => etching.Id.Equals(someEtching.Id));
			Assert.Equal(@gathering.Id, serverSomeEtching.GatheringId);
			Assert.Equal(someEtching.Id, serverSomeEtching.Id);
		}

		[Fact]
		public async Task GetUserFeedAsync_ExcludingGathering_ReturnsFeed()
		{
			// Arrange
			var host1 = await environment.GenerateUniqueUserAsync();
			var host2 = await environment.GenerateUniqueUserAsync();
			var friend = await environment.GenerateUniqueUserAsync();
			await environment.ForceFriendshipAsync(host1, host2, friend);

			var gathering1 = await environment.GenerateOngoingGatheringAsync(host1);
			var seenEtching = await environment.GenerateEtchingAsync(gathering1, host1);

			var gathering2 = await environment.GeneratePastGatheringAsync(host2);
			var unseenEtching = await environment.GenerateEtchingAsync(gathering1, host2);

			// Act
			var feed = await director.GetUserFeedAsync(friend.Id, 100, 1);

			// Assert
			Assert.Single(feed.Headers);
			Assert.Equal(gathering2.Id, feed.Headers[0].Id);

			Assert.Single(feed.Etchings);
			var serverSomeEtching = feed.Etchings.Find(etching => etching.Id.Equals(unseenEtching.Id));
			Assert.Equal(gathering2.Id, serverSomeEtching.GatheringId);
			Assert.Equal(unseenEtching.Id, serverSomeEtching.Id);
		}
	}
}
