using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

    public record ImageMetadataShard(string Hash, bool Concealed);

    #endregion

    #region Gates

    public interface IMediaDatabase
    {
        Task<MemoryStream> DownloadAssetAsync(string asset);

        Task<MemoryStream> DownloadAvatarAsync(long userId);
        Task UploadAvatarAsync(long userId, MemoryStream image);
        Task DeleteAvatarAsync(long userId);

        Task<MemoryStream> DownloadHeroAsync(long gatheringId);
        Task UploadHeroAsync(long gatheringId, MemoryStream image);
        Task DeleteHeroAsync(long gatheringId);

        Task<MemoryStream> DownloadSnapshotAsync(long snapshotId, long ownerId);
        Task UploadSnapshotAsync(long snapshotId, long ownerId, MemoryStream image);
        Task DeleteSnapshotAsync(long snapshotId, long ownerId);
    }

    public interface IMediaOperations
    {
        Task<MemoryStream> GetAssetAsync(string asset);
        Task<MemoryStream> GetAvatarAsync(long userId, long targetId);
        Task<ImageMetadataShard> GetAvatarMetadataAsync(long userId, long targetId);
        Task<MemoryStream> GetHeaderAsync(long userId, long gatheringId);
        Task<ImageMetadataShard> GetHeaderMetadataAsync(long userId, long gatheringId);
        Task<MemoryStream> GetSnapshotAsync(long userId, long snapshotId);
        Task<ImageMetadataShard> GetSnapshotMetadataAsync(long userId, long snapshotId);
    }

    #endregion
}
