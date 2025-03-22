using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class ConnectionDirector : AbstractDirector, IConnectionOperations
    {
        #region Initialisation

        public ConnectionDirector(CoreTerminal terminal) : base(terminal) { }

        #endregion

        #region Operations

        public async Task UserConnectedAsync(long userId, string connectionId)
        {
            await Connections.AddConnectionAsync(userId, connectionId);
        }

        public async Task UserDisconnectedAsync(long userId, string connectionId)
        {
            await Connections.DeleteConnectionAsync(connectionId);
        }

        #endregion

        #region Favours


        #endregion
    }
}
