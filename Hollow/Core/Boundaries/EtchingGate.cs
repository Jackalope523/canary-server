using System;
using Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    public record EventHeader(ulong Id, string Name, bool IsActive, DateTimeOffset LastActiveTime);

    public record Etching(ulong Id, ulong EventId, ulong UserId,
        DateTimeOffset TimeEtched, string ImageURL,
        (int Positive, int Negative) Ratings);

    public interface IEtchingDatabase
    {
        List<Etching> GetEtchingsForEvent(ulong id);
        List<Etching> GetEtchingsByUser(ulong id);
        Etching GetEtching(ulong id);
        Etching AddEtching(ulong eventId, ulong etcherId,
            DateTimeOffset timeEtched, string imageURL);
        bool RemoveEtching(ulong etchingId);

        bool RateEtching(ulong etchingId, ulong voterId, UserRating rating);
        bool RemoveEtchingRating(ulong etchingId, ulong voterId);

        List<Etching> GenerateFeedForUser(ulong id, DateTimeOffset depthCharge, List<ulong> exclusionList);
    }

    public interface IEtchingOperations
    {
        Task<List<Etching>> GetEventEtchingsAsync(ulong userID, ulong eventID);
        Task<Etching> AddEtchingAsync(ulong userID, ulong eventID, string imageURL);
        Task RemoveEtchingAsync(ulong userID, ulong etchingID);
        Task RateEtchingAsync(ulong userID, ulong etchingID, UserRating rating);

        Task<(int Depth, List<EventHeader> Headers, List<Etching> Etchings)>
            GetUserFeedAsync(ulong userID, int depth,
            List<ulong> exclusionList = null);
    }
}

