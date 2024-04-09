using Azure.Security.KeyVault.Keys;
using Core.Boundaries;

namespace Repository.Coordinators
{
    public class KeyStoreCoordinator : IKeyDatabase
    {
        IKeyDatabase store;

        public KeyStoreCoordinator()
        {
            store = new AzureKeyStore();
        }

        public Task<byte[]> GetCertificateAsync(string certificateName)
        {
            return store.GetCertificateAsync(certificateName);
        }

        public Task<JsonWebKey> GetKeyAsync(string keyName)
        {
            return store.GetKeyAsync(keyName);
        }

        public Task<string> GetSecretAsync(string secretName)
        {
            return store.GetSecretAsync(secretName);
        }
    }
}
