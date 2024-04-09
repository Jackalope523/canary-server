using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Certificates;
using Core.Boundaries;
using Repository.KeyVaults.AzureKeyVault.Sentries;

namespace Repository
{
    public class AzureKeyStore : IKeyDatabase
    {
        AzureKeySentry sentry;

        public AzureKeyStore()
        {
            sentry = new AzureKeySentry();
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            return await sentry.GetSecretAsync(secretName);
        }

        public async Task<JsonWebKey> GetKeyAsync(string keyName)
        {
            return await sentry.GetKeyAsync(keyName);
        }

        public async Task<byte[]> GetCertificateAsync(string certificateName)
        {
            return await sentry.GetCertificateAsync(certificateName);
        }
    }
}
