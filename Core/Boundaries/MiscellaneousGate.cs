using System;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IMiscellaneousDatabase
    {
		Task SaveFeedback(string name, DateTimeOffset time, string comments);
    }

    public interface IMiscellaneousOperations
    {
		Task ReceiveFeedback(ulong userId, string comments);
		Task ReceiveAnonymousFeedback(string pseudonym, string comments);
	}

    #endregion
}

