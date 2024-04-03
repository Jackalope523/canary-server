using Azure.Identity;
using Azure.Storage.Blobs;
using Shared;

namespace Repository
{
    public class AzureStorageSentry : IStorageSentry
    {
        private readonly AzureStorageContext storageContext;

        public AzureStorageSentry()
        {
            storageContext = new AzureStorageContext();
        }
        public async Task UploadBlobAsync(string containerName, string blobName, MemoryStream stream)
        {
            BlobContainerClient containerClient = new(storageContext.GetUri(containerName), new DefaultAzureCredential());

            try
            {
                await containerClient.CreateIfNotExistsAsync();

                stream.Position = 0;
                await containerClient.UploadBlobAsync(blobName, stream);
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }

        public async Task<MemoryStream> DownloadBlobAsync(string containerName, string blobName)
        {
            BlobClient blobClient = new(storageContext.GetUri(containerName, blobName), new DefaultAzureCredential());
            MemoryStream stream = new MemoryStream();
            try
            {
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
            finally
            {
                stream.Dispose();
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
