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
            await sentry.DeleteBlobAsync(ownerId.ToString(), snapshotId.ToString());
        }

        public async Task<MemoryStream> DownloadAvatarAsync(ulong userId)
        {
            return await sentry.DownloadBlobAsync(userId.ToString(), "");
        }

        public async Task UploadAvatarAsync(ulong userId, MemoryStream image)
        {
            await sentry.UploadBlobAsync(userId.ToString(), "", image);
        }

        public async Task DeleteAvatarAsync(ulong userId)
        {
            await sentry.DeleteBlobAsync(userId.ToString(), "");
        }

        public async Task<MemoryStream> DownloadHeroAsync(ulong gatheringId)
        {
            return await sentry.DownloadBlobAsync(gatheringId.ToString(), "");
        }

        public async Task UploadHeroAsync(ulong gatheringId, MemoryStream image)
        {
            await sentry.UploadBlobAsync(gatheringId.ToString(), "", image);
        }

        public async Task DeleteHeroAsync(ulong gatheringId)
        {
            await sentry.DeleteBlobAsync(gatheringId.ToString(), "");
        }
    }
}
