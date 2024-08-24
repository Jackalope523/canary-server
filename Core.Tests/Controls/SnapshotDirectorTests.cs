using Core.Boundaries;
using Core.Controls;
using Core.Entities;

using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Controls
{
    public class SnapshotDirectorTests : CoreTest
    {
		private SnapshotDirector director;

        public SnapshotDirectorTests()
        {
			director = environment.Terminal.SnapshotDirector;
        }

		[Fact]
		public async Task GetGatheringSnapshotsAsync_HostedGathering_ReturnsSnapshots()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateUpcomingGatheringAsync(host);
			await environment.GenerateSnapshotAsync(gathering, host);
			await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			var serverSnapshots = await director.GetGatheringSnapshotsAsync(host.Id, gathering.Id);

			// Assert
			Assert.Equal(2, serverSnapshots.Count);
		}

		[Fact]
		public async Task GetGatheringSnapshotsAsync_GuestAtGathering_ReturnsSnapshots()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateOngoingGatheringAsync(host, guest);
			await environment.GenerateSnapshotAsync(gathering, host);
			await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			var serverSnapshots = await director.GetGatheringSnapshotsAsync(guest.Id, gathering.Id);

			// Assert
			Assert.Equal(2, serverSnapshots.Count);
		}

		[Fact]
		public async Task GetGatheringSnapshotsAsync_InvalidGathering_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateUpcomingGatheringAsync(host);
			await environment.GenerateSnapshotAsync(gathering, host);
			await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			var serverSnapshots = director.GetGatheringSnapshotsAsync(sneakyUser.Id, gathering.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await serverSnapshots);
		}

		[Fact]
		public async Task AddSnapshotAsync_GuestAtGathering_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateOngoingGatheringAsync(host, guest);
			byte[] image = { byte.MinValue, 0, 1, 3, byte.MaxValue, 7, 8 };

			// Act
			await director.AddSnapshotAsync(guest.Id, gathering.Id, new(image));

			// Assert
			var serverSnapshots = await director.GetGatheringSnapshotsAsync(guest.Id, gathering.Id);
			Assert.Single(serverSnapshots);

			var snapshot = serverSnapshots[0];
			Assert.Equal(image, (await environment.Terminal.MediaDatabase.DownloadSnapshotAsync(snapshot.Id, guest.Id)).ToArray());
		}

		[Fact]
		public async Task AddSnapshotAsync_InvalidGathering_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateUpcomingGatheringAsync(host);

			// Act
			var addSnapshotSync = director.AddSnapshotAsync(sneakyUser.Id, gathering.Id, new(0));

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await addSnapshotSync);
		}

		[Fact]
		public async Task RemoveSnapshotAsync_OwnedSnapshot_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateUpcomingGatheringAsync(host);
			var snapshot = await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			await director.DeleteSnapshotAsync(host.Id, snapshot.Id);

			// Assert
			var serverSnapshots = await director.GetGatheringSnapshotsAsync(host.Id, gathering.Id);
			Assert.Empty(serverSnapshots);
		}

		[Fact]
		public async Task RemoveSnapshotAsync_UnownedSnapshot_Fails()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var sneakyUser = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateUpcomingGatheringAsync(host);
			var snapshot = await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			var removeSnapshotSync = director.DeleteSnapshotAsync(sneakyUser.Id, snapshot.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await removeSnapshotSync);
		}

		[Fact]
		public async Task RateSnapshotAsync_ValidSnapshot_Succeeds()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var guest = await environment.GenerateUniqueUserAsync();
			var gathering = await environment.GenerateOngoingGatheringAsync(host, guest);
			var coolSnapshot = await environment.GenerateSnapshotAsync(gathering, host);
			var uglySnapshot = await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			await director.AcclaimSnapshotAsync(guest.Id, coolSnapshot.Id, SnapshotAcclaim.Acclaim);

			// Assert
			var serverSnapshots = await director.GetGatheringSnapshotsAsync(host.Id, gathering.Id);
			
			var serverCoolSnapshot = serverSnapshots.Find(snapshot => snapshot.Id.Equals(coolSnapshot.Id));
			Assert.Equal(1, serverCoolSnapshot.Acclaim);

			var serverUglySnapshot = serverSnapshots.Find(snapshot => snapshot.Id.Equals(uglySnapshot.Id));
			Assert.Equal(0, serverUglySnapshot.Acclaim);
		}

		[Fact]
		public async Task GetUserColumnAsync_ReturnsColumn()
		{
			// Arrange
			var host = await environment.GenerateUniqueUserAsync();
			var companion = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(host, companion);

			var gathering = await environment.GeneratePastGatheringAsync(host);
			var someSnapshot = await environment.GenerateSnapshotAsync(gathering, host);
			var anotherSnapshot = await environment.GenerateSnapshotAsync(gathering, host);

			// Act
			var column = await director.GetUserColumnAsync(companion.Id, 100, 0);

			// Assert
			Assert.Single(column.Headers);
			Assert.Equal(gathering.Id, column.Headers[0].Id);

			Assert.Equal(2, column.Snapshots.Count);
			var serverSomeSnapshot = column.Snapshots.Find(snapshot => snapshot.Id.Equals(someSnapshot.Id));
			Assert.Equal(gathering.Id, serverSomeSnapshot.GatheringId);
			Assert.Equal(someSnapshot.Id, serverSomeSnapshot.Id);
		}

		[Fact]
		public async Task GetUserColumnAsync_ExcludingGathering_ReturnsColumn()
		{
			// Arrange
			var host1 = await environment.GenerateUniqueUserAsync();
			var host2 = await environment.GenerateUniqueUserAsync();
			var companion = await environment.GenerateUniqueUserAsync();
			await environment.ForceCompanionshipAsync(host1, host2, companion);

			var gathering1 = await environment.GenerateOngoingGatheringAsync(host1);
			var seenSnapshot = await environment.GenerateSnapshotAsync(gathering1, host1);

			var gathering2 = await environment.GeneratePastGatheringAsync(host2);
			var unseenSnapshot = await environment.GenerateSnapshotAsync(gathering1, host2);

			// Act
			var column = await director.GetUserColumnAsync(companion.Id, 100, 1);

			// Assert
			Assert.Single(column.Headers);
			Assert.Equal(gathering2.Id, column.Headers[0].Id);

			Assert.Single(column.Snapshots);
			var serverSomeSnapshot = column.Snapshots.Find(snapshot => snapshot.Id.Equals(unseenSnapshot.Id));
			Assert.Equal(gathering2.Id, serverSomeSnapshot.GatheringId);
			Assert.Equal(unseenSnapshot.Id, serverSomeSnapshot.Id);
		}
	}
}
