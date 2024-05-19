using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
	#region Schemas

	public record GatheringHeader(ulong Id, string Name, bool IsActive, DateTimeOffset LastActiveTime,
        double Latitude, double Longitude);

    public record EtchingShard(ulong Id, ulong GatheringId, UserSilhouette User,
        DateTimeOffset TimeEtched,
        (int Positive, int Negative) Ratings, bool IsHidden);

    public record FeedShard(List<GatheringHeader> Headers, List<EtchingShard> Etchings);

    #endregion

    #region Gates

    public interface IEtchingDatabase
    {
        Task<List<EtchingShard>> GetEtchingsForGatheringAsync(ulong gatheringId);
        Task<List<EtchingShard>> GetEtchingsByUserAsync(ulong userId);
        Task<EtchingShard> GetEtchingAsync(ulong etchingId);
        Task<EtchingShard> AddEtchingAsync(ulong gatheringId, ulong etcherId,
            DateTimeOffset timeEtched);
		Task RemoveEtchingAsync(ulong etchingId);
		Task HideEtchingAsync(ulong etchingId);

		Task RateEtchingAsync(ulong etchingId, ulong voterId, UserRating rating);
		Task RemoveEtchingRatingAsync(ulong etchingId, ulong voterId);

        Task<List<EtchingShard>> GenerateFeedForUserAsync(ulong userId, DateTimeOffset depthCharge, DateTimeOffset lastDepth);
    }

    public interface IEtchingOperations
    {
        Task<List<EtchingShard>> GetGatheringEtchingsAsync(ulong userId, ulong gatheringId);
        Task<EtchingShard> AddEtchingAsync(ulong userId, ulong gatheringId, MemoryStream image);
        Task RemoveEtchingAsync(ulong userId, ulong etchingId);
        Task RateEtchingAsync(ulong userId, ulong etchingId, UserRating rating);

        Task<FeedShard> GetUserFeedAsync(ulong userId, int depth, int lastDepth);
    }

	#endregion
}

