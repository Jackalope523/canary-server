using Core.Boundaries;
using Serilog;

namespace Repository
{
    public class AzureFileStore : IMediaDatabase
    {
        private readonly AzureStorageSentry sentry = new();

        public async Task<MemoryStream> DownloadAssetAsync(string asset)
        {
            return await sentry.DownloadBlobAsync("assets", asset);
        }

        public async Task UploadSnapshotAsync(ulong snapshotId, ulong ownerId, MemoryStream image)
        {
            await sentry.UploadBlobAsync("user" + ownerId.ToString(), snapshotId.ToString(), image);
        }

        public async Task<MemoryStream> DownloadSnapshotAsync(ulong snapshotId, ulong ownerId)
        {
            return await sentry.DownloadBlobAsync("user" + ownerId.ToString(), snapshotId.ToString());
        }

        public async Task DeleteSnapshotAsync(ulong snapshotId, ulong ownerId)
        {
            await sentry.DeleteBlobAsync("user" + ownerId.ToString(), snapshotId.ToString());
        }

        public async Task<MemoryStream> DownloadAvatarAsync(ulong userId)
        {
            return await sentry.DownloadBlobAsync("user" + userId.ToString(), "avatar");
        }

        public async Task UploadAvatarAsync(ulong userId, MemoryStream image)
        {
            await sentry.UploadBlobAsync("user" + userId.ToString(), "avatar", image);
        }

        public async Task DeleteAvatarAsync(ulong userId)
        {
            await sentry.DeleteBlobAsync("user" + userId.ToString(), "avatar");
        }

        public async Task<MemoryStream> DownloadHeroAsync(ulong gatheringId)
        {
            return await sentry.DownloadBlobAsync("gathering" + gatheringId.ToString(), "hero");
        }

        public async Task UploadHeroAsync(ulong gatheringId, MemoryStream image)
        {
            await sentry.UploadBlobAsync("gathering" + gatheringId.ToString(), "hero", image);
        }

        public async Task DeleteHeroAsync(ulong gatheringId)
        {
            await sentry.DeleteBlobAsync("gathering" + gatheringId.ToString(), "hero");
        }
    }
}
