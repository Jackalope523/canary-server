using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
	#region Schemas

	public record GatheringHeader(ulong Id, string Name, bool IsActive, DateTimeOffset LastActiveTime,
        double Latitude, double Longitude);

    public record SnapshotShard(ulong Id, ulong GatheringId, UserSilhouette User,
        DateTimeOffset TimeEtched,
        (int Positive, int Negative) Ratings, bool IsHidden);

    public record FeedShard(List<GatheringHeader> Headers, List<SnapshotShard> Snapshots);

    #endregion

    #region Gates

    public interface ISnapshotDatabase
    {
        Task<List<SnapshotShard>> GetSnapshotsForGatheringAsync(ulong gatheringId);
        Task<List<SnapshotShard>> GetSnapshotsByUserAsync(ulong userId);
        Task<SnapshotShard> GetSnapshotAsync(ulong snapshotId);
        Task<SnapshotShard> AddSnapshotAsync(ulong gatheringId, ulong etcherId,
            DateTimeOffset timeEtched);
		Task RemoveSnapshotAsync(ulong snapshotId);
		Task HideSnapshotAsync(ulong snapshotId);

		Task AcclaimSnapshotAsync(ulong snapshotId, ulong voterId, UserRating rating);
		Task RemoveSnapshotAcclaimAsync(ulong snapshotId, ulong voterId);

        Task<List<SnapshotShard>> GenerateFeedForUserAsync(ulong userId, DateTimeOffset depthCharge, DateTimeOffset lastDepth);
    }

    public interface ISnapshotOperations
    {
        Task<List<SnapshotShard>> GetGatheringSnapshotsAsync(ulong userId, ulong gatheringId);
        Task<SnapshotShard> AddSnapshotAsync(ulong userId, ulong gatheringId, MemoryStream image);
        Task RemoveSnapshotAsync(ulong userId, ulong snapshotId);
        Task AcclaimSnapshotAsync(ulong userId, ulong snapshotId, UserRating rating);

        Task<FeedShard> GetUserFeedAsync(ulong userId, int depth, int lastDepth);
    }

	#endregion
}

