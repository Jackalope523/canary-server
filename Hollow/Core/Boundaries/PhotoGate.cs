using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IImageDatabase
    {
        public Task<MemoryStream> DownloadImageAsync(ulong etchingId, ulong ownerId);
        public Task UploadImageAsync(ulong etchingId, ulong ownerId, Image image);
        public Task DeleteImageAsync(ulong etchingId, ulong ownerId);
    }

    #endregion
}
