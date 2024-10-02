using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IKeyDatabase
    {
		Task<string> GetHollowOneSignalApiKeyAsync();
		Task<string> GetHollowOneSignalAppIdAsync();

		Task<string> GetHollowTwilioAccountKeyAsync();
		Task<string> GetHollowTwilioAuthTokenAsync();
		Task<string> GetHollowTwilioMessagingServiceAsync();

		Task<string> GetCanaryMapKeyAsync();
    }

    public interface IKeyOperations
	{
		Task<string> GetCanaryMapKeyAsync(ulong userId);
	}

    #endregion
}

