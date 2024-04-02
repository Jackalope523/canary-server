namespace Repository
{
    public interface IStorageSentry
    {
        public Task UploadBlobAsync(string containerName, string blobName, byte[] blob);
        public Task<byte[]> DownloadBlobAsync(string containerName, string blobName);
        public Task DeleteBlobAsync(string containerName, string blobName);
    }
}



