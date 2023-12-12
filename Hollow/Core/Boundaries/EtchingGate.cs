using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    public record EventHeader(Guid Id, string Name, bool IsActive, DateTimeOffset LastActiveTime);

    public record Etching(Guid Id, Guid EventId, Guid UserId,
        DateTimeOffset TimeEtched, string ImageURL,
        (int Positive, int Negative) Ratings);

    public interface IEtchingDatabase
    {
        Task<List<Etching>> GetEtchingsForEventAsync(Guid id);
        Task<List<Etching>> GetEtchingsByUserAsync(Guid id);
        Task<Etching> GetEtchingAsync(Guid id);
        Task<Etching> AddEtchingAsync(Guid eventId, Guid etcherId, DateTimeOffset timeEtched, string imageURL);
        Task<bool> RemoveEtchingAsync(Guid etchingId);
        Task<bool> RateEtchingAsync(Guid etchingId, Guid voterId, UserRating rating);
        Task<bool> RemoveEtchingRatingAsync(Guid etchingId, Guid voterId);
        Task<List<Etching>> GenerateFeedForUserAsync(Guid id, DateTimeOffset depthCharge, List<Guid> exclusionList);
    }

    public interface IEtchingOperations
    {
        Task<List<Etching>> GetEventEtchingsAsync(Guid userID, Guid eventID);
        Task<Etching> AddEtchingAsync(Guid userID, Guid eventID, string imageURL);
        Task RemoveEtchingAsync(Guid userID, Guid etchingID);
        Task RateEtchingAsync(Guid userID, Guid etchingID, UserRating rating);

        Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(Guid userID, int depth,
            List<Guid> exclusionList = null);
    }
}

