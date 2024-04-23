using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IMediaDatabase
    {
        Task<MemoryStream> DownloadImageAsync(ulong etchingId, ulong ownerId);
        Task UploadImageAsync(ulong etchingId, ulong ownerId, MemoryStream image);
        Task DeleteImageAsync(ulong etchingId, ulong ownerId);
    }

    public interface IMediaOperations
    {
        Task<MemoryStream> GetImageStreamAsync(ulong userId, ulong etchingId);
    }

    #endregion
}
