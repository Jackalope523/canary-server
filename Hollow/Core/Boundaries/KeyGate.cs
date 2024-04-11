using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IKeyDatabase
    {
        Task<string> GetSecretAsync(string secretName);
        Task<object> GetKeyAsync(string keyName);
        Task<byte[]> GetCertificateAsync(string keyName);
    }

    public interface IKeyOperations
	{
		Task<string> GetSecretAsync(ulong userId, string secretName);
	}

    #endregion
}

