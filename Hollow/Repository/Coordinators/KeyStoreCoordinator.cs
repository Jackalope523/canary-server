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

        public async Task<byte[]> GetCertificateAsync(string certificateName)
        {
            return await store.GetCertificateAsync(certificateName);
        }

        public async Task<object> GetKeyAsync(string keyName)
        {
            return await store.GetKeyAsync(keyName);
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            return await store.GetSecretAsync(secretName);
        }
    }
}
