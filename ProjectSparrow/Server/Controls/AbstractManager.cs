using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Boundaries;
using Server.Entities;
using Shared;

namespace Server.Controls
{
	internal abstract class AbstractManager
	{
		protected CoreTerminal Terminal { get; init; }

		protected IAccountDatabase Accounts { get; private set; }
		protected IEventDatabase Events { get; private set; }
		protected IPostDatabase Posts { get; private set; }
		protected IProfileDatabase Profiles { get; private set; }
		protected IReportDatabase Reports { get; private set; }

		public AbstractManager(CoreTerminal terminal)
		{
			Terminal = terminal;
		}

		internal void Bridge()
		{
			Accounts = Terminal.AccountDatabase;
			Events = Terminal.EventDatabase;
			Posts = Terminal.PostDatabase;
			Profiles = Terminal.ProfileDatabase;
			Reports = Terminal.ReportDatabase;
        }

        internal async Task<User> GetUser(Guid userID)
        {
            User user = new(Accounts.FindUserById(userID));

            // Check if user account is locked
            if (user.IsLocked)
            { throw new InvalidUserException("User account is locked."); }

            return user;
        }

        internal async Task<Event> GetEvent(Guid eventID)
        {
            return new(Events.FindEvent(eventID));
        }
    }
}

