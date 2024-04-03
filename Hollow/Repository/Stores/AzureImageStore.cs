using Core.Boundaries;
using System.Drawing;

namespace Repository
{
    internal class AzureImageStore : IImageDatabase
    {
        private IStorageSentry sentry;

        public AzureImageStore()
        {
            sentry = new AzureStorageSentry();
        }
        
        public async Task<MemoryStream> DownloadImageAsync(ulong etchingId, ulong ownerId)
        {
            return await sentry.DownloadBlobAsync(ownerId.ToString(), etchingId.ToString());
        }

        public async Task UploadImageAsync(ulong etchingId, ulong ownerId, MemoryStream image)
        {
            await sentry.UploadBlobAsync(ownerId.ToString(), etchingId.ToString(), image);
        }

        public async Task DeleteImageAsync(ulong etchingId, ulong ownerId)
        {
            await sentry.DeleteBlobAsync(ownerId.ToString(), etchingId.ToString());
        }
    }
}
