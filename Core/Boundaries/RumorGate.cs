using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schema
    public record CoreRumoredGathering(long Id, double Latitude, double Longitude, string FriendlyLocation) : CoreOnlyData();
    public record CoreRumor(long Id, string Text, DateTimeOffset Time) : CoreOnlyData();

    public record RumorGatheringShard();
    public record RumorShard();


    #endregion

    #region Gates

    public interface IRumorDatabase
    {
        Task<CoreRumoredGathering> CreateRumoredGatheringAsync(double latitude, double longitude, string friendlyLocation);
        Task<CoreRumoredGathering> GetRumoredGatheringAsync(long id);
        Task<UserShard> GetFounderAsync(long rumoredGatheringId);
        Task SoftDeleteRumoredGatheringAsync(long id);
        Task HardDeleteRumoredGatheringAsync(long id);
        Task UpdateRumoredGatheringAsync(long id, List<(string Property, object Value)> edits);

        Task<CoreRumor> CreateRumorAsync(long rumoredGatheringId, long authorId, string text, DateTimeOffset time);
        Task<CoreRumor> GetRumorAsync(long id);
        Task<List<CoreRumor>> GetRumorsAboutAsync(long rumoredGatheringId);
        Task<List<CoreRumor>> GetRumorsByAsync(long userId);
        Task<List<CoreRumor>> GetRumorsByCompanionsOfAsync(long userId);
        Task<List<(CoreRumor, string)>> GetWallRumorsAsync(long userId);
        Task SoftDeleteRumorAsync(long id);
        Task HardDeleteRumorAsync(long id);
        Task UpdateRumorAsync(long id, List<(string Property, object Value)> edits);
    }

    public interface IRumorOperations
    {

    }

    #endregion
}