using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IMediaDatabase
    {
        Task<MemoryStream> DownloadImageAsync(ulong snapshotId, ulong ownerId);
        Task UploadImageAsync(ulong snapshotId, ulong ownerId, MemoryStream image);
        Task DeleteImageAsync(ulong snapshotId, ulong ownerId);
    }

    public interface IMediaOperations
    {
        Task<MemoryStream> GetImageStreamAsync(ulong userId, ulong snapshotId);
    }

    #endregion
}
