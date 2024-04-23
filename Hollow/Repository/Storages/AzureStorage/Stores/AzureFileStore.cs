using Core.Boundaries;

namespace Repository
{
    public class AzureFileStore : IMediaDatabase
    {
        private readonly AzureStorageSentry sentry = new();

        public async Task UploadImageAsync(ulong etchingId, ulong ownerId, MemoryStream image)
        {
            await sentry.UploadBlobAsync(ownerId.ToString(), etchingId.ToString(), image);
        }

        public async Task<MemoryStream> DownloadImageAsync(ulong etchingId, ulong ownerId)
        {
            return await sentry.DownloadBlobAsync(ownerId.ToString(), etchingId.ToString());
        }

        public async Task DeleteImageAsync(ulong etchingId, ulong ownerId)
        {
            await sentry.DownloadBlobAsync(ownerId.ToString(), etchingId.ToString());
        }
    }
}
