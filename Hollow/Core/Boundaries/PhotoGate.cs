using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IPhotoDatabase
    {
        public Task<byte[]> DownloadPhotoAsync(ulong etchingId, ulong ownerId);
        public Task UploadPhotoAsync(ulong etchingId, ulong ownerId, byte[] blob);
        public Task DeletePhotoAsync(ulong etchingId, ulong ownerId);
    }

    #endregion
}
