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
		protected IDisciplineDatabase Reports { get; private set; }
		protected IMediaDatabase Media { get; private set; }
		protected INotificationDatabase Notifications { get; private set; }
		protected IProfileDatabase Profiles { get; private set; }

		#endregion

		#region Initialisation

		public AbstractDirector(CoreTerminal terminal)
		{
			Terminal = terminal;
			
			Accounts = Terminal.AccountDatabase;
			Events = Terminal.EventDatabase;
			Etchings = Terminal.EtchingDatabase;
			Reports = Terminal.DisciplineDatabase;
			Media = Terminal.MediaDatabase;
			Notifications = Terminal.NotificationDatabase;
			Profiles = Terminal.ProfileDatabase;
        }

		#endregion

		#region Tools

		protected async Task<User> GetUserAsync(ulong userId)
        {
            User user = new(await Accounts.FindUserByIdAsync(userId));

			// Fail if user account is locked
			Fail(user.IsLocked,
				new InvalidUserException("User account is locked."));

			// Fail if user account is pending deletion
			Fail(user.IsDeleted,
				new InvalidUserException("User account is deleted"));

            return user;
        }

		protected async Task<User> GetUserUnsafeAsync(ulong userId)
        {
            return new(await Accounts.FindUserByIdAsync(userId));
        }

        protected async Task<Event> GetEventAsync(ulong eventId)
        {
            return new(await Events.FindEventAsync(eventId));
        }

		#endregion
	}
}

