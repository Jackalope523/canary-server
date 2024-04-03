using Azure.Identity;

namespace Repository
{
    public class AzureStorageContext
    {
        public readonly string storageAccountName = "sparrowstorageaccount";
        public readonly string baseUrl = "https://{0}.blob.core.windows.net";
        public readonly Func<Azure.Core.TokenCredential> credentials = () => new DefaultAzureCredential();

        public Uri GetUri(string containerName)
        {
            return new Uri(string.Format(baseUrl + "/{1}", storageAccountName, containerName));
        }
        public Uri GetUri(string containerName, string blobName)
        {
            return new Uri(string.Format(baseUrl + "/{1}/{2}", storageAccountName, containerName, blobName));
        }
    }
}
