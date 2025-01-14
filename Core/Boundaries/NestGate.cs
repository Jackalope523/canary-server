using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    public record NestShard(List<TwigShard> Twigs,
        long RelativeGatheringId = default);
    public record TwigShard(long GatheringId, DateTimeOffset StartTime);

    public record AgendaShard(List<CardShard> Cards);
    public record CardShard(long GatheringId, DateTimeOffset StartTime, GatheringBond Bond);

    public record BlockedUserShard(long Id, string NameWhenBlocked, DateTimeOffset DateBlocked) :
        UserShard(Id, NameWhenBlocked);

	#endregion

	#region Gates

	public interface INestDatabase
    {
        Task<List<CoreUser>> GetCompanionsAsync(long userId);
		Task<List<CoreUser>> GetAppreciatedUsersAsync(long userId);
        Task<List<CoreUser>> GetUsersAppreciatingAsync(long userId);
        Task<List<BlockedUserShard>> GetBlockedUsersAsync(long userId);
        Task<List<CoreUser>> GetUsersBlockingAsync(long userId);

        Task AppreciateUserAsync(long userId, long targetId, DateTimeOffset time);
		Task UnappreciateUserAsync(long userId, long targetId);
		Task BlockUserAsync(long userId, long targetId, DateTimeOffset time);
		Task UnblockUserAsync(long userId, long targetId);

        Task<bool> HaveMutualGathering(long userId, long targetId);
        Task<CoreGathering> GetFirstMutualGathering(long userId, long targetId);
        Task<CoreGathering> GetLatestMutualGathering(long userId, long targetId);
        Task<DateTimeOffset> BlockedSince(long userId, long targetId);
    }

	public interface INestOperations
    {
        Task<NestShard> GetNestAsync(long userId, long targetId);

        Task<AgendaShard> GetUserAgendaAsync(long userId);
        Task<IDictionary<long, AgendaShard>> GetCompanionAgendasAsync(long userId);

        Task<List<UserShard>> GetCompanionsAsync(long userId);
        Task<List<UserShard>> GetAppreciatedUsersAsync(long userId);
        Task<List<BlockedUserShard>> GetBlockedUsersAsync(long userId);

        Task AppreciateUserAsync(long userId, long targetId);
        Task UnappreciateUserAsync(long userId, long targetId);
        Task BlockUserAsync(long userId, long targetId);
        Task UnblockUserAsync(long userId, long targetId);

        Task<bool> AuthorisedToAppreciate(long userId, long targetId);
    }

	#endregion
}

