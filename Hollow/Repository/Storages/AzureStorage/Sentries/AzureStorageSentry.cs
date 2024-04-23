using Azure.Storage.Blobs;
using Shared;

namespace Repository
{
    internal class AzureStorageSentry
    {
        AzureStorageContext context = new();


        public async Task UploadBlobAsync(string containerName, string blobName, MemoryStream blob)
        {
            BlobContainerClient containerClient = new(context.BuildUri(containerName), context.credentials());

            try
            {
                await containerClient.CreateIfNotExistsAsync();

                blob.Position = 0;
                await containerClient.UploadBlobAsync(blobName, blob);
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }

        public async Task<MemoryStream> DownloadBlobAsync(string containerName, string blobName)
        {
            BlobClient blobClient = new(context.BuildUri(containerName, blobName), context.credentials());
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
            BlobClient blobClient = new(context.BuildUri(containerName, blobName), context.credentials());

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
