using Core.Boundaries;
using Serilog;

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

        public async Task<object> GetKeyAsync(string keyName)
        {
            return await sentry.GetKeyAsync(keyName);
        }

        public async Task<byte[]> GetCertificateAsync(string certificateName)
        {
            return await sentry.GetCertificateAsync(certificateName);
        }
    }
}
