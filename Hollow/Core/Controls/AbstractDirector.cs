using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
	internal abstract class AbstractDirector
	{
		#region Variables

		protected CoreTerminal Terminal { get; init; }

		protected IAccountDatabase Accounts { get; private set; }
		protected IEventDatabase Events { get; private set; }
		protected IEtchingDatabase Etchings { get; private set; }
		protected IProfileDatabase Profiles { get; private set; }
		protected IReportDatabase Reports { get; private set; }
		protected INotificationDatabase Notifications { get; private set; }

		#endregion

		#region Initialisation

		public AbstractDirector(CoreTerminal terminal)
		{
			Terminal = terminal;
			
			Accounts = Terminal.AccountDatabase;
			Events = Terminal.EventDatabase;
			Etchings = Terminal.EtchingDatabase;
			Profiles = Terminal.ProfileDatabase;
			Reports = Terminal.ReportDatabase;
			Notifications = Terminal.NotificationDatabase;
        }

		#endregion

		#region Tools

		protected async Task<User> GetUser(ulong userId)
        {
            User user = new(Accounts.FindUserById(userId));

			// Check if user account is locked
			Fail(user.IsLocked,
				new InvalidUserException("User account is locked."));

            return user;
        }

		protected async Task<User> GetUserUnsafe(ulong userId)
        {
            return new(Accounts.FindUserById(userId));
        }

        protected async Task<Event> GetEvent(ulong eventId)
        {
            return new(Events.FindEvent(eventId));
        }

		#endregion
	}
}

