using Azure.Identity;
using Azure.Storage.Blobs;
using Shared;
using System.Drawing;
using System.Drawing.Imaging;

namespace Repository
{
    public class AzureStorageSentry : IStorageSentry
    {
        private readonly AzureStorageContext storageContext;

        public AzureStorageSentry()
        {
            storageContext = new AzureStorageContext();
        }
        public async Task UploadBlobAsync(string containerName, string blobName, Image image)
        {
            BlobContainerClient containerClient = new(storageContext.GetUri(containerName), new DefaultAzureCredential());

            try
            {
                await containerClient.CreateIfNotExistsAsync();

                using (MemoryStream stream = new())
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    stream.Position = 0;

                    await containerClient.UploadBlobAsync(blobName, stream);
                }
            }
            catch (Exception ex)
            {
                throw new BlobIOException(ex);
            }
        }

        public async Task<MemoryStream> DownloadBlobAsync(string containerName, string blobName)
        {
            BlobClient blobClient = new(storageContext.GetUri(containerName, blobName), new DefaultAzureCredential());

            try
            {
                using (MemoryStream stream = new())
                {
                    await blobClient.DownloadToAsync(stream);
                    stream.Position = 0;
                    return stream;  
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
