using Azure.Identity;

namespace Repository
{
    internal class AzureKeyVaultContext
    {
        public Uri Uri = new Uri("https://sparrowkeys.vault.azure.net/");
        public readonly Func<Azure.Core.TokenCredential> credentials = () => new DefaultAzureCredential();
    }
}
