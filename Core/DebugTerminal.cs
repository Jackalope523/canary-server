using System;
using System.Collections.Generic;
using System.Threading;
using Core.Controls;
using Core.Boundaries;
using Microsoft.Extensions.Logging;

namespace Core
{
    public class DebugTerminal : CoreTerminal
	{
		#region Variables

		private static object initLock = new();

		public IDebugDatabase DebugDatabase { get; init; }

		public IDebugOperations DebugOperations
			=> DebugDirector;

		internal DebugDirector DebugDirector { get; private set; }

		#endregion

		#region Initialisation

		public static DebugTerminal CreateDebugTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IGatheringDatabase gatheringDatabase, ISnapshotDatabase snapshotDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            INestDatabase nestDatabase,
            INotificationService notificationService, IDebugDatabase debugDatabase)
		{
			lock (initLock)
			{
				Terminal ??= new DebugTerminal(logger,
                        accountDatabase, adminDatabase, bannerDatabase,
                        gatheringDatabase, snapshotDatabase,
                        disciplineDatabase, keyDatabase,
                        mediaDatabase, notificationDatabase,
                        nestDatabase,
                        notificationService,
						debugDatabase);

				return (DebugTerminal) Terminal;
			}
		}

		protected DebugTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IGatheringDatabase gatheringDatabase, ISnapshotDatabase snapshotDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            INestDatabase nestDatabase,
            INotificationService notificationService, IDebugDatabase debugDatabase)
			: base(logger,
					accountDatabase, adminDatabase, bannerDatabase,
					gatheringDatabase, snapshotDatabase,
					disciplineDatabase, keyDatabase,
					mediaDatabase, notificationDatabase,
					nestDatabase,  notificationService)
		{
			DebugDatabase = debugDatabase;
			DebugDirector = new DebugDirector(this);
		}

		#endregion
	}
}