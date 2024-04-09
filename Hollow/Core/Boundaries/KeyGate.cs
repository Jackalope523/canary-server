using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IKeyDatabase
    {
        public Task<string> GetSecretAsync(string secretName);
        public Task<JsonWebKey> GetKeyAsync(string keyName);
        public Task<byte[]> GetCertificateAsync(string keyName);
    }

    public interface IKeyOperations
    {

    }

    #endregion
}

