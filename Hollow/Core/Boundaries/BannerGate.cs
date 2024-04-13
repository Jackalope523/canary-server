using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schema

	#endregion

	#region Gates

	public interface IBannerDatabase
	{
        Task<string> GetUserBannerAsync(ulong userId);
        Task<string> GetUserBannerAsync(string phoneNumber);
        Task AddBannerMemberAsync(string phoneNumber, string banner);
    }

	public interface IBannerOperations
	{
        Task InviteUserAsync(ulong userId, string invitedPhoneNumber);
    }

    #endregion
}

