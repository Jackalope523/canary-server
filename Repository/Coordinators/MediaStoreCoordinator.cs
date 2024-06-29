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
    }
}
