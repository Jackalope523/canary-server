using Core.Boundaries;

namespace Repository
{
    internal class MediaStoreCoordinator: IImageDatabase
    {
        private readonly IImageDatabase store;

        public MediaStoreCoordinator()
        {
            store = new AzureImageStore();
        }
        
        public async Task<MemoryStream> DownloadImageAsync(ulong etchingId, ulong ownerId)
        {
            return await store.DownloadImageAsync(etchingId, ownerId);
        }

        public async Task UploadImageAsync(ulong etchingId, ulong ownerId, MemoryStream image)
        {
            await store.UploadImageAsync(etchingId, ownerId, image);
        }

        public async Task DeleteImageAsync(ulong etchingId, ulong ownerId)
        {
            await store.DeleteImageAsync(etchingId, ownerId);
        }
    }
}
