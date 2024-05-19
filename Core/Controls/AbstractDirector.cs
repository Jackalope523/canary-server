using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

using static Core.Entities.Arbiter;

namespace Core.Controls
{
    internal abstract class AbstractDirector
	{
		#region Variables

		protected CoreTerminal Terminal { get; init; }

		protected IAccountDatabase Accounts { get; private set; }
		protected IBannerDatabase Banners { get; private set; }
		protected IGatheringDatabase Gatherings { get; private set; }
		protected ISnapshotDatabase Snapshots { get; private set; }
		protected IDisciplineDatabase Reports { get; private set; }
		protected IKeyDatabase Keys { get; private set; }
		protected IMediaDatabase Media { get; private set; }
		protected INotificationDatabase Notifications { get; private set; }
		protected IProfileDatabase Profiles { get; private set; }

		#endregion

		#region Initialisation

		public AbstractDirector(CoreTerminal terminal)
		{
			Terminal = terminal;
			
			Accounts = Terminal.AccountDatabase;
			Banners = Terminal.BannerDatabase;
			Gatherings = Terminal.GatheringDatabase;
			Snapshots = Terminal.SnapshotDatabase;
			Reports = Terminal.DisciplineDatabase;
			Keys = Terminal.KeyDatabase;
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

        protected async Task<Gathering> GetGatheringAsync(ulong gatheringId)
        {
            return new(await Gatherings.FindGatheringAsync(gatheringId));
        }

		#endregion
	}
}

