namespace Repository
{
    internal class MediaStoreCoordinator: IMediaDatabase
    {
        private readonly IMediaDatabase store;

        public MediaStoreCoordinator()
        {
            store = new AzureFileStore();
        }
        
        public async Task<MemoryStream> DownloadSnapshotAsync(ulong snapshotId, ulong ownerId)
        {
            return await store.DownloadSnapshotAsync(snapshotId, ownerId);
        }

        public async Task UploadSnapshotAsync(ulong snapshotId, ulong ownerId, MemoryStream image)
        {
            await store.UploadSnapshotAsync(snapshotId, ownerId, image);
        }

        public async Task DeleteSnapshotAsync(ulong snapshotId, ulong ownerId)
        {
            await store.DeleteSnapshotAsync(snapshotId, ownerId);
        }

        public async Task<MemoryStream> DownloadAvatarAsync(ulong userId)
        {
            return await store.DownloadAvatarAsync(userId);
        }

        public async Task UploadAvatarAsync(ulong userId, MemoryStream image)
        {
            await store.UploadAvatarAsync(userId, image);
        }

        public async Task DeleteAvatarAsync(ulong userId)
        {
            await store.DeleteAvatarAsync(userId);
        }

        public async Task<MemoryStream> DownloadHeroAsync(ulong gatheringId)
        {
            return await store.DownloadHeroAsync(gatheringId);
        }

        public async Task UploadHeroAsync(ulong gatheringId, MemoryStream image)
        {
            await store.UploadHeroAsync(gatheringId, image);
        }

        public async Task DeleteHeroAsync(ulong gatheringId)
        {
            await store.DeleteHeroAsync(gatheringId);
        }
    }
}
