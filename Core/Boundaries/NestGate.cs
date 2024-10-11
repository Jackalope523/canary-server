using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public record NestShard(List<TwigShard> Twigs,
        ulong RelativeGatheringId = default, DateTimeOffset BlockedSince = default);
    public record TwigShard(ulong GatheringId, DateTimeOffset StartTime);

    public record AgendaShard(List<CardShard> Cards);
    public record CardShard(ulong GatheringId, DateTimeOffset StartTime, GatheringBond Bond);

	#endregion

	#region Gates

	public interface INestDatabase
    {
        Task<List<UserShard>> GetCompanionsAsync(ulong userId);
		Task<List<UserShard>> GetAppreciatedUsersAsync(ulong userId);
        Task<List<UserShard>> GetUsersAppreciatingAsync(ulong userId);
        Task<List<UserShard>> GetBlockedUsersAsync(ulong userId);
        Task<List<UserShard>> GetUsersBlockingAsync(ulong userId);

        Task AppreciateUserAsync(ulong userId, ulong targetId, DateTimeOffset time);
		Task UnappreciateUserAsync(ulong userId, ulong targetId);
		Task BlockUserAsync(ulong userId, ulong targetId, DateTimeOffset time);
		Task UnblockUserAsync(ulong userId, ulong targetId);

        Task<bool> HaveMutualGathering(ulong userId, ulong targetId);
        Task<CoreGathering> GetFirstMutualGathering(ulong userId, ulong targetId);
        Task<CoreGathering> GetLatestMutualGathering(ulong userId, ulong targetId);
        Task<DateTimeOffset> BlockedSince(ulong userId, ulong targetId);
    }

	public interface INestOperations
    {
        Task<NestShard> GetNestAsync(ulong userId, ulong targetId);

        Task<AgendaShard> GetUserAgendaAsync(ulong userId);
        Task<IDictionary<ulong, AgendaShard>> GetCompanionAgendasAsync(ulong userId);

        Task<List<UserShard>> GetCompanionsAsync(ulong userId);
        Task<List<UserShard>> GetAppreciatedUsersAsync(ulong userId);
        Task<List<UserShard>> GetBlockedUsersAsync(ulong userId);

        Task AppreciateUserAsync(ulong userId, ulong targetId);
        Task UnappreciateUserAsync(ulong userId, ulong targetId);
        Task BlockUserAsync(ulong userId, ulong targetId);
        Task UnblockUserAsync(ulong userId, ulong targetId);

        Task<bool> AuthorisedToAppreciate(ulong userId, ulong targetId);
    }

	#endregion
}

