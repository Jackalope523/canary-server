namespace Repository
{
    public interface IBlobStorageSentry
    {
        public Task UploadBlob(string accountName, string containerName, string blobName, string blobContents);
        public Task<Stream> DownloadBlobAsync(string containerName, string blobName);
        public Task DeleteBlobAsync(string containerName, string blobName);
        public Task<bool> BlobExistsAsync(string containerName, string blobName);
        public Task<IEnumerable<string>> ListBlobsAsync(string containerName);
    }
}



