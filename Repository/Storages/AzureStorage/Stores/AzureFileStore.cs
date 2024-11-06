using Core.Boundaries;
using Serilog;

namespace Repository
{
    public class AzureFileStore : IMediaDatabase
    {
        private readonly AzureStorageSentry sentry;

        public AzureFileStore(Harbor.Flag flag) 
        {
            sentry = new(flag);
        }

        public async Task<MemoryStream> DownloadAssetAsync(string asset)
        {
            return await sentry.DownloadBlobAsync("assets", asset);
        }

        public async Task UploadSnapshotAsync(long snapshotId, long ownerId, MemoryStream image)
        {
            await sentry.UploadBlobAsync("users" + ownerId.ToString(), snapshotId.ToString(), image);
        }

        public async Task<MemoryStream> DownloadSnapshotAsync(long snapshotId, long ownerId)
        {
            return await sentry.DownloadBlobAsync("users" + ownerId.ToString(), snapshotId.ToString());
        }

        public async Task DeleteSnapshotAsync(long snapshotId, long ownerId)
        {
            await sentry.DeleteBlobAsync("users" + ownerId.ToString(), snapshotId.ToString());
        }

        public async Task<MemoryStream> DownloadAvatarAsync(long userId)
        {
            return await sentry.DownloadBlobAsync("user" + userId.ToString(), "avatar");
        }

        public async Task UploadAvatarAsync(long userId, MemoryStream image)
        {
            await sentry.UploadBlobAsync("user" + userId.ToString(), "avatar", image);
        }

        public async Task DeleteAvatarAsync(long userId)
        {
            await sentry.DeleteBlobAsync("user" + userId.ToString(), "avatar");
        }

        public async Task<MemoryStream> DownloadHeroAsync(long gatheringId)
        {
            return await sentry.DownloadBlobAsync("gathering" + gatheringId.ToString(), "hero");
        }

        public async Task UploadHeroAsync(long gatheringId, MemoryStream image)
        {
            await sentry.UploadBlobAsync("gathering" + gatheringId.ToString(), "hero", image);
        }

        public async Task DeleteHeroAsync(long gatheringId)
        {
            await sentry.DeleteBlobAsync("gathering" + gatheringId.ToString(), "hero");
        }
    }
}
