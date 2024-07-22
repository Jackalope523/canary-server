using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IMediaDatabase
    {
        Task<MemoryStream> DownloadAssetAsync(string asset);

        Task<MemoryStream> DownloadAvatarAsync(ulong userId);
        Task UploadAvatarAsync(ulong userId, MemoryStream image);
        Task DeleteAvatarAsync(ulong userId);

        Task<MemoryStream> DownloadHeroAsync(ulong gatheringId);
        Task UploadHeroAsync(ulong gatheringId, MemoryStream image);
        Task DeleteHeroAsync(ulong gatheringId);

        Task<MemoryStream> DownloadSnapshotAsync(ulong snapshotId, ulong ownerId);
        Task UploadSnapshotAsync(ulong snapshotId, ulong ownerId, MemoryStream image);
        Task DeleteSnapshotAsync(ulong snapshotId, ulong ownerId);
    }

    public interface IMediaOperations
    {
        Task<MemoryStream> GetAssetAsync(string asset);
        Task<MemoryStream> GetAvatarAsync(ulong userId, ulong targetId);
        Task<MemoryStream> GetHeroAsync(ulong userId, ulong gatheringId);
        Task<MemoryStream> GetSnapshotAsync(ulong userId, ulong snapshotId);
    }

    #endregion
}
