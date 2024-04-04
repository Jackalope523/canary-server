using System.Drawing;

namespace Repository
{
    public interface IStorageSentry
    {
        public Task UploadBlobAsync(string containerName, string blobName, MemoryStream blob);
        public Task<MemoryStream> DownloadBlobAsync(string containerName, string blobName);
        public Task DeleteBlobAsync(string containerName, string blobName);
    }
}



