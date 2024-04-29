using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Gates

	public interface IDebugDatabase
	{
        Task DrainDatabaseAsync();
    }

	public interface IDebugOperations
	{
        Task AddUserToBannerAsync(string phoneNumber, string banner);
        Task SeedDatabaseAsync();
    }

    #endregion
}

