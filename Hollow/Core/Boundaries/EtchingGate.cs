using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

	public record EventHeader(ulong Id, string Name, bool IsActive, DateTimeOffset LastActiveTime,
        double Latitude, double Longitude);

    public record Etching(ulong Id, ulong EventId, UserSilhouette User,
        DateTimeOffset TimeEtched, string ImageURL,
        (int Positive, int Negative) Ratings, bool IsHidden);

	#endregion

	#region Gates

	public interface IEtchingDatabase
    {
        Task<List<Etching>> GetEtchingsForEventAsync(ulong eventId);
        Task<List<Etching>> GetEtchingsByUserAsync(ulong userId);
        Task<Etching> GetEtchingAsync(ulong etchingId);
        Task<Etching> AddEtchingAsync(ulong eventId, ulong etcherId,
            DateTimeOffset timeEtched, string imageURL);
		Task RemoveEtchingAsync(ulong etchingId);
		Task HideEtchingAsync(ulong etchingId);

		Task RateEtchingAsync(ulong etchingId, ulong voterId, UserRating rating);
		Task RemoveEtchingRatingAsync(ulong etchingId, ulong voterId);

        Task<List<Etching>> GenerateFeedForUserAsync(ulong userId, DateTimeOffset depthCharge, List<ulong> exclusionList);
    }

    public interface IEtchingOperations
    {
        Task<List<Etching>> GetEventEtchingsAsync(ulong userId, ulong eventId);
        Task<Etching> AddEtchingAsync(ulong userId, ulong eventId, string imageURL);
        Task RemoveEtchingAsync(ulong userId, ulong etchingId);
        Task RateEtchingAsync(ulong userId, ulong etchingId, UserRating rating);

        Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(ulong userId, int depth,
            List<ulong> exclusionList = null);
    }

	#endregion
}

