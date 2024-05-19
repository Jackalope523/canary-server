using Core.Boundaries;

namespace Repository
{
    internal class MediaStoreCoordinator: IMediaDatabase
    {
        private readonly IMediaDatabase store;

        public MediaStoreCoordinator()
        {
            store = new AzureFileStore();
        }
        
        public async Task<MemoryStream> DownloadImageAsync(ulong snapshotId, ulong ownerId)
        {
            return await store.DownloadImageAsync(snapshotId, ownerId);
        }

        public async Task UploadImageAsync(ulong snapshotId, ulong ownerId, MemoryStream image)
        {
            await store.UploadImageAsync(snapshotId, ownerId, image);
        }

        public async Task DeleteImageAsync(ulong snapshotId, ulong ownerId)
        {
            await store.DeleteImageAsync(snapshotId, ownerId);
        }
    }
}
