using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Shared;

namespace Core.Controls
{
	internal abstract class AbstractDirector
	{
		protected CoreTerminal Terminal { get; init; }

		protected IAccountDatabase Accounts { get; private set; }
		protected IEventDatabase Events { get; private set; }
		protected IEtchingDatabase Etchings { get; private set; }
		protected IProfileDatabase Profiles { get; private set; }
		protected IReportDatabase Reports { get; private set; }

		public AbstractDirector(CoreTerminal terminal)
		{
			Terminal = terminal;
		}

		internal void Bridge()
		{
			Accounts = Terminal.AccountDatabase;
			Events = Terminal.EventDatabase;
			Etchings = Terminal.EtchingDatabase;
			Profiles = Terminal.ProfileDatabase;
			Reports = Terminal.ReportDatabase;
        }

        internal async Task<User> GetUser(Guid userID)
        {
            User user = new(await Accounts.FindUserByIdAsync(userID));

            // Check if user account is locked
            if (user.IsLocked)
            { throw new InvalidUserException("User account is locked."); }

            return user;
        }

        internal async Task<Event> GetEvent(Guid eventID)
        {
            return new(await Events.FindEventAsync(eventID));
        }
    }
}

