using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schema

    public record CoreOnlyData();

    #endregion

    #region Gates

    public interface IAdminDatabase
	{
        Task<List<CoreGathering>> GetAllWaitingGatheringsAsync(DateTimeOffset currentTime);
        Task VoidUserAsync(ulong userId);
        Task VoidGatheringAsync(ulong gatheringId);
    }

    #endregion
}

