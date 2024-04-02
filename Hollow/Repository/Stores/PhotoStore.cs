using Core.Boundaries;
using Shared;

namespace Repository
{
    internal class PhotoStore : IPhotoDatabase
    {
        private IStorageSentry sentry;

        public PhotoStore(IStorageSentry sentry)
        {
            this.sentry = sentry;
        }
        
        public async Task<byte[]> DownloadPhotoAsync(ulong etchingId, ulong ownerId)
        {
            return await sentry.DownloadBlobAsync(ownerId.ToString(), etchingId.ToString());
        }

        public async Task UploadPhotoAsync(ulong etchingId, ulong ownerId, byte[] blob)
        {
            await sentry.UploadBlobAsync(ownerId.ToString(), etchingId.ToString(), blob);
        }

        public async Task DeletePhotoAsync(ulong etchingId, ulong ownerId)
        {
            await sentry.DeleteBlobAsync(ownerId.ToString(), etchingId.ToString());
        }
    }
}
