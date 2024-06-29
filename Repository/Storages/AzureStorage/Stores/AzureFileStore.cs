using Core.Boundaries;

namespace Repository
{
    public class AzureFileStore : IMediaDatabase
    {
        private readonly AzureStorageSentry sentry = new();

        public async Task UploadSnapshotAsync(ulong snapshotId, ulong ownerId, MemoryStream image)
        {
            await sentry.UploadBlobAsync(ownerId.ToString(), snapshotId.ToString(), image);
        }

        public async Task<MemoryStream> DownloadSnapshotAsync(ulong snapshotId, ulong ownerId)
        {
            return await sentry.DownloadBlobAsync(ownerId.ToString(), snapshotId.ToString());
        }

        public async Task DeleteSnapshotAsync(ulong snapshotId, ulong ownerId)
        {
            await sentry.DownloadBlobAsync(ownerId.ToString(), snapshotId.ToString());
        }
    }
}
