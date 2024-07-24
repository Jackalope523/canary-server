using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
	#region Schemas

    public enum SnapshotAcclaim
    { Acclaim, Remove }

	public record GatheringHeader(ulong Id, string Name, bool IsActive, DateTimeOffset LastActiveTime,
        string FriendlyLocation);

    public record SnapshotShard(ulong Id, ulong GatheringId, UserShard User,
        DateTimeOffset TimeTaken, int Acclaim);

    public record ColumnShard(List<GatheringHeader> Headers, List<SnapshotShard> Snapshots);

    #endregion

    #region Gates

    public interface ISnapshotDatabase
    {
        Task<List<SnapshotShard>> GetSnapshotsForGatheringAsync(ulong gatheringId);
        Task<List<SnapshotShard>> GetSnapshotsByUserAsync(ulong userId);
        Task<SnapshotShard> GetSnapshotAsync(ulong snapshotId);
        Task<SnapshotShard> AddSnapshotAsync(ulong gatheringId, ulong etcherId,
            DateTimeOffset timeTaken);
		Task RemoveSnapshotAsync(ulong snapshotId);

		Task AcclaimSnapshotAsync(ulong snapshotId, ulong voterId);
		Task RemoveSnapshotAcclaimAsync(ulong snapshotId, ulong voterId);

        Task<List<SnapshotShard>> GenerateColumnForUserAsync(ulong userId, DateTimeOffset depthCharge, DateTimeOffset lastDepth);
    }

    public interface ISnapshotOperations
    {
        Task<List<SnapshotShard>> GetGatheringSnapshotsAsync(ulong userId, ulong gatheringId);
        Task<SnapshotShard> AddSnapshotAsync(ulong userId, ulong gatheringId, MemoryStream image);
        Task RemoveSnapshotAsync(ulong userId, ulong snapshotId);
        Task AcclaimSnapshotAsync(ulong userId, ulong snapshotId, SnapshotAcclaim acclaim);

        Task<ColumnShard> GetUserColumnAsync(ulong userId, int depth, int lastDepth);
    }

	#endregion
}

