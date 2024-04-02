using Azure.Identity;
using Azure.Storage.Blobs;
using Shared;

namespace Repository
{
    public class AzureStorageSentry : IStorageSentry
    {
        private readonly AzureStorageContext storageContext;

        public AzureStorageSentry(Harbor.Flag flag)
        {
            storageContext = new AzureStorageContext();
        }
        public async Task UploadBlobAsync(string containerName, string blobName, byte[] blob)
        {
            BlobContainerClient containerClient = new(storageContext.GetUri(containerName), new DefaultAzureCredential());

            try
            {
                await containerClient.CreateIfNotExistsAsync();

                using (MemoryStream stream = new(blob))
                {
                    await containerClient.UploadBlobAsync(blobName, stream);
                }
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }

        public async Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
        {
            BlobClient blobClient = new(storageContext.GetUri(containerName, blobName), new DefaultAzureCredential());

            try
            {
                using (MemoryStream stream = new())
                {
                    await blobClient.DownloadToAsync(stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobClient blobClient = new(storageContext.GetUri(containerName, blobName), new DefaultAzureCredential());

            try
            {
                 await blobClient.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }
    }
}
