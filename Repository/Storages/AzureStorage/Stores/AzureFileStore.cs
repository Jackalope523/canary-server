using Core.Boundaries;

namespace Repository
{
    public class AzureFileStore : IMediaDatabase
    {
        private readonly AzureStorageSentry sentry = new();

        public async Task UploadImageAsync(ulong snapshotId, ulong ownerId, MemoryStream image)
        {
            await sentry.UploadBlobAsync(ownerId.ToString(), snapshotId.ToString(), image);
        }

        public async Task<MemoryStream> DownloadImageAsync(ulong snapshotId, ulong ownerId)
        {
            return await sentry.DownloadBlobAsync(ownerId.ToString(), snapshotId.ToString());
        }

        public async Task DeleteImageAsync(ulong snapshotId, ulong ownerId)
        {
            await sentry.DownloadBlobAsync(ownerId.ToString(), snapshotId.ToString());
        }
    }
}
