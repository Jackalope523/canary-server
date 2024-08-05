using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IKeyDatabase
    {
		Task<string> GetHollowTwilioAuthKeyAsync();
		Task<string> GetHollowTwilioTokenKeyAsync();

		Task<string> GetSparrowMapKeyAsync();
    }

    public interface IKeyOperations
	{
		Task<string> GetHollowTwilioAuthKeyAsync();
		Task<string> GetHollowTwilioTokenKeyAsync();

		Task<string> GetSparrowMapKeyAsync(ulong userId);
	}

    #endregion
}

