using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal class KeyDirector : AbstractDirector, IKeyOperations
	{
		#region Initialisation

		public KeyDirector(CoreTerminal terminal) : base(terminal) { }

        #endregion

        #region Operations

        public async Task<string> GetHollowTwilioAuthKeyAsync()
        {
            return await Keys.GetHollowTwilioAuthKeyAsync();
        }

        public async Task<string> GetHollowTwilioTokenKeyAsync()
        {
            return await Keys.GetHollowTwilioTokenKeyAsync();
        }

        public async Task<string> GetCanaryMapKeyAsync(ulong userId)
        {
            await GetUserAsync(userId);

            return await Keys.GetCanaryMapKeyAsync();
        }

        #endregion

        #region Favours


        #endregion
    }
}
