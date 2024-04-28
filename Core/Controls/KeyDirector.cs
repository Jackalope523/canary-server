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

		public async Task<string> GetSecretAsync(ulong userId, string secret)
		{
			await GetUserAsync(userId);

			return await Keys.GetSecretAsync(secret);
		}

		#endregion

		#region Favours


		#endregion
	}
}
