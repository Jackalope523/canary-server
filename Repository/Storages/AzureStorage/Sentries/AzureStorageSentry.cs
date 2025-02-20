using Azure.Storage.Blobs;

namespace Repository
{
    internal class AzureStorageSentry
    {
        AzureStorageContext context;

        public AzureStorageSentry(Harbor.Flag flag)
        {
            context = new(flag);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, MemoryStream blob)
        {
            BlobContainerClient containerClient = new(context.BuildUri(containerName), context.credentials());

            try
            {
                await containerClient.CreateIfNotExistsAsync();
                blob.Position = 0;

                await containerClient.GetBlobClient(blobName + ".jpg").UploadAsync(blob, overwrite: true);
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }

        public async Task<MemoryStream> DownloadBlobAsync(string containerName, string blobName)
        {
            MemoryStream stream = new MemoryStream();

            BlobClient blobClient = new(context.BuildUri(containerName, blobName + ".jpg"), context.credentials());

            if (!(await blobClient.ExistsAsync()))
            {
                BlobClient repairClient = new(context.BuildUri(containerName, blobName), context.credentials());
                await repairClient.DownloadToAsync(stream);
                stream.Position = 0;
                await repairClient.DeleteAsync();
                await blobClient.UploadAsync(stream);
            }

            try
            {
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                return await DownloadBlobAsync("utility", "failed_image.jpg");
            }
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobClient blobClient = new(context.BuildUri(containerName, blobName + ".jpg"), context.credentials());

            if (!(await blobClient.ExistsAsync()))
            {
                blobClient = new(context.BuildUri(containerName, blobName), context.credentials());
            }

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
