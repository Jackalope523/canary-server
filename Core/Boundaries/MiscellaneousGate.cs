using System;
using System.Threading.Tasks;

namespace Core.Boundaries
{
    #region Schemas

    #endregion

    #region Gates

    public interface IMiscellaneousDatabase
    {
		Task SaveFeedbackAsync(string comments, DateTimeOffset time);
        Task SaveFeedbackAsync(string comments, DateTimeOffset time, ulong userId);
    }

    public interface IMiscellaneousOperations
    {
		Task ReceiveFeedback(ulong userId, string comments);
		Task ReceiveAnonymousFeedback(ulong userId, string pseudonym, string comments);
	}

    #endregion
}

