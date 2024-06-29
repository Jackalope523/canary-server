using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IMediaDatabase
    {
        Task<MemoryStream> DownloadSnapshotAsync(ulong snapshotId, ulong ownerId);
        Task UploadSnapshotAsync(ulong snapshotId, ulong ownerId, MemoryStream image);
        Task DeleteSnapshotAsync(ulong snapshotId, ulong ownerId);

        Task<MemoryStream> DownloadAvatarAsync(ulong userId);
        Task UploadAvatarAsync(ulong userId, MemoryStream image);
        Task DeleteAvatarAsync(ulong userId);
    }

    public interface IMediaOperations
    {
        Task<MemoryStream> GetAvatarAsync(ulong userId, ulong targetId);
        Task<MemoryStream> GetSnapshotAsync(ulong userId, ulong snapshotId);
    }

    #endregion
}
