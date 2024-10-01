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

		Task<string> GetHollowTwilioAuthKeyAsync();
		Task<string> GetHollowTwilioTokenKeyAsync();

		Task<string> GetCanaryMapKeyAsync();
    }

    public interface IKeyOperations
	{
		Task<string> GetHollowTwilioAuthKeyAsync();
		Task<string> GetHollowTwilioTokenKeyAsync();

		Task<string> GetCanaryMapKeyAsync(ulong userId);
	}

    #endregion
}

