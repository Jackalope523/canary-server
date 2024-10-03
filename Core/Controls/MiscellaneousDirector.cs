using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;

using static Core.Entities.Psijic;

namespace Core.Controls
{
    internal class MiscellaneousDirector : AbstractDirector, IMiscellaneousOperations
    {
		#region Initialisation

		public MiscellaneousDirector(CoreTerminal terminal) : base(terminal) { }

        #endregion

        #region Operations

        public async Task ReceiveFeedback(ulong userId, string comments)
        {
            var user = await GetUserAsync(userId);

            await Miscellaneous.SaveFeedback($"{user.Id} {user.Name}", Time, comments);
        }

        public async Task ReceiveAnonymousFeedback(string pseudonym, string comments)
        {
            await Miscellaneous.SaveFeedback(pseudonym, Time, comments);
        }

        #endregion

        #region Favours


        #endregion
    }
}
