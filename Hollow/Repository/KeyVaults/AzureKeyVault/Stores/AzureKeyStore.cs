using Azure.Security.KeyVault.Keys;
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
            using var log = new LoggerConfiguration()
                            .WriteTo.AzureApp()
                            .CreateLogger();
            
            log.Fatal("Logger Initialized Properly");

            var secret = await sentry.GetSecretAsync(secretName);

            log.Fatal("Retrieved secret: {$secret}", secret);

            return secret;
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
