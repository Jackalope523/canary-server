using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace Repository
{
    internal class AzureKeySentry
    {
        AzureKeyVaultContext context = new();

        public async Task<string> GetSecretAsync(string secretName)
        {
            SecretClient client = new(context.Uri, context.credentials());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }

        public async Task<JsonWebKey> GetKeyAsync(string keyName)
        {
            KeyClient client = new(context.Uri, context.credentials());
            KeyVaultKey key = await client.GetKeyAsync(keyName);
            return key.Key;
        }

        public async Task<byte[]> GetCertificateAsync(string certificateName)
        {
            CertificateClient client = new(context.Uri, context.credentials());
            KeyVaultCertificate certificate = await client.GetCertificateAsync(certificateName);
            return certificate.Cer;
        }
    }
}
