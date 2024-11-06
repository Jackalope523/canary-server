namespace Repository
{
    internal class MediaStoreCoordinator: IMediaDatabase
    {
        private readonly IMediaDatabase store;

        public MediaStoreCoordinator(Harbor.Flag flag)
        {
            store = new AzureFileStore(flag);
        }

        public async Task<MemoryStream> DownloadAssetAsync(string asset)
        {
            return await store.DownloadAssetAsync(asset);
        }
        
        public async Task<MemoryStream> DownloadSnapshotAsync(long snapshotId, long ownerId)
        {
            return await store.DownloadSnapshotAsync(snapshotId, ownerId);
        }

        public async Task UploadSnapshotAsync(long snapshotId, long ownerId, MemoryStream image)
        {
            await store.UploadSnapshotAsync(snapshotId, ownerId, image);
        }

        public async Task DeleteSnapshotAsync(long snapshotId, long ownerId)
        {
            await store.DeleteSnapshotAsync(snapshotId, ownerId);
        }

        public async Task<MemoryStream> DownloadAvatarAsync(long userId)
        {
            return await store.DownloadAvatarAsync(userId);
        }

        public async Task UploadAvatarAsync(long userId, MemoryStream image)
        {
            await store.UploadAvatarAsync(userId, image);
        }

        public async Task DeleteAvatarAsync(long userId)
        {
            await store.DeleteAvatarAsync(userId);
        }

        public async Task<MemoryStream> DownloadHeroAsync(long gatheringId)
        {
            return await store.DownloadHeroAsync(gatheringId);
        }

        public async Task UploadHeroAsync(long gatheringId, MemoryStream image)
        {
            await store.UploadHeroAsync(gatheringId, image);
        }

        public async Task DeleteHeroAsync(long gatheringId)
        {
            await store.DeleteHeroAsync(gatheringId);
        }
    }
}
