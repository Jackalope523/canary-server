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
        List<Etching> GetEtchingsForEvent(ulong eventId);
        List<Etching> GetEtchingsByUser(ulong userId);
        Etching GetEtching(ulong etchingId);
        Etching AddEtching(ulong eventId, ulong etcherId,
            DateTimeOffset timeEtched, string imageURL);
        bool RemoveEtching(ulong etchingId);

        bool RateEtching(ulong etchingId, ulong voterId, UserRating rating);
        bool RemoveEtchingRating(ulong etchingId, ulong voterId);

        List<Etching> GenerateFeedForUser(ulong userId, DateTimeOffset depthCharge, List<ulong> exclusionList);
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
}

