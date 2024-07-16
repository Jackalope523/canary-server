using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public record NestShard(List<GatheringShard> Gatherings, List<SnapshotShard> Snapshots);
    public record AgendaShard(List<AgendaBondPair> Agenda);

    public record AgendaBondPair(GatheringShard Gathering, GatheringBond Bond);

	#endregion

	#region Gates

	public interface INestDatabase
    {
        Task<List<UserShard>> GetCompanionsAsync(ulong userId);
		Task<List<UserShard>> GetAppreciatedUsersAsync(ulong userId);
        Task<List<UserShard>> GetUsersAppreciatingAsync(ulong userId);
        Task<List<UserShard>> GetBlockedUsersAsync(ulong userId);
        Task<List<UserShard>> GetUsersBlockingAsync(ulong userId);

        Task AppreciateUserAsync(ulong userId, ulong targetUserId, DateTimeOffset time);
		Task UnappreciateUserAsync(ulong userId, ulong targetUserId);
		Task BlockUserAsync(ulong userId, ulong targetUserId, DateTimeOffset time);
		Task UnblockUserAsync(ulong userId, ulong targetUserId);
    }

	public interface INestOperations
    {
        Task<NestShard> GetUserNestAsync(ulong userId, ulong targetId);

        Task<AgendaShard> GetUserAgendaAsync(ulong userId);
        Task<IDictionary<UserShard, AgendaShard>> GetCompanionAgendasAsync(ulong userId);

        Task<List<UserShard>> GetCompanionsAsync(ulong userId);
        Task<List<UserShard>> GetAppreciatedUsersAsync(ulong userId);
        Task<List<UserShard>> GetBlockedUsersAsync(ulong userId);

        Task AppreciateUserAsync(ulong userId, ulong targetId);
        Task UnappreciateUserAsync(ulong userId, ulong targetId);
        Task BlockUserAsync(ulong userId, ulong targetId);
        Task UnblockUserAsync(ulong userId, ulong targetId);
    }

	#endregion
}

