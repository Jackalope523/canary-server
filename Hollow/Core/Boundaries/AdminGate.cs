using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Gates

	public interface IAdminDatabase
	{
        Task VoidUserAsync(ulong userId);
        Task VoidEventAsync(ulong eventId);
    }

    #endregion
}

