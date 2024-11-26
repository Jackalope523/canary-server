using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Microsoft.Extensions.Logging;
using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal abstract class AbstractDirector
	{
		#region Variables

		protected CoreTerminal Terminal { get; init; }

		protected EnvironmentOptions Environment { get; init; }

		protected ILogger Log { get; private set; }

		protected IAccountDatabase Accounts { get; private set; }
		protected IGatheringDatabase Gatherings { get; private set; }
		protected ISnapshotDatabase Snapshots { get; private set; }
		protected IDisciplineDatabase Reports { get; private set; }
		protected IKeyDatabase Keys { get; private set; }
		protected IMediaDatabase Media { get; private set; }
		protected INotificationDatabase Telegrams { get; private set; }
		protected INestDatabase Nests { get; private set; }
        protected IMiscellaneousDatabase Miscellaneous { get; private set; }

        #endregion

        #region Initialisation

        public AbstractDirector(CoreTerminal terminal)
		{
			Terminal = terminal;
			Environment = terminal.Environment;

			Log = Terminal.Log;
			
			Accounts = Terminal.AccountDatabase;
			Gatherings = Terminal.GatheringDatabase;
			Snapshots = Terminal.SnapshotDatabase;
			Reports = Terminal.DisciplineDatabase;
			Keys = Terminal.KeyDatabase;
			Media = Terminal.MediaDatabase;
			Telegrams = Terminal.NotificationDatabase;
			Nests = Terminal.NestDatabase;
			Miscellaneous = Terminal.MiscellaneousDatabase;
        }

		#endregion

		#region Tools

		protected async Task<User> GetUserAsync(long userId)
        {
            User user = new(await Accounts.FindUserByIdAsync(userId));
			
			// Fail if user account is locked
			FailIf(user.IsLocked,
				new InvalidUserException("User account is locked."));

			// Fail if user account is pending deletion
			FailIf(user.IsDeleted,
				new InvalidUserException("User account is deleted"));

            return user;
        }

		protected async Task<User> GetUserUnsafeAsync(long userId)
        {
            return new(await Accounts.FindUserByIdAsync(userId));
        }

        protected async Task<Gathering> GetGatheringAsync(long gatheringId)
        {
            return new(await Gatherings.FindGatheringAsync(gatheringId));
        }

		#endregion
	}
}

