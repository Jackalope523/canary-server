using System;
using System.Collections.Generic;
using System.Threading;
using Core.Controls;
using Microsoft.Extensions.Logging;

namespace Core.Boundaries
{
    public class DebugTerminal : CoreTerminal
	{
		#region Variables

		public static new CoreTerminal Terminal { get; private set; }
		private static object initLock = new();

		public IDebugDatabase DebugDatabase { get; init; }

		public IDebugOperations DebugOperations
			=> DebugDirector;

		internal DebugDirector DebugDirector { get; private set; }

		public new List<(Type GateType, object Instance)> Gates
			=> new() { (typeof(IAccountOperations), AccountOperations),
				(typeof(IEventOperations), EventOperations),
				(typeof(IEtchingOperations), EtchingOperations),
				(typeof(IProfileOperations), ProfileOperations),
				(typeof(IDisciplineOperations), DisciplineOperations),
				(typeof(INotificationOperations), NotificationOperations),
				(typeof(IDebugOperations), DebugOperations) };

		#endregion

		#region Initialisation

		public static CoreTerminal CreateDebugTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IEventDatabase eventDatabase, IEtchingDatabase etchingDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            IProfileDatabase profileDatabase,
            INotificationService notificationService, IDebugDatabase debugDatabase)
		{
			lock (initLock)
			{
				if (Terminal == null)
				{
					Terminal = new DebugTerminal(logger,
                        accountDatabase, adminDatabase, bannerDatabase,
                        eventDatabase, etchingDatabase,
                        disciplineDatabase, keyDatabase,
                        mediaDatabase, notificationDatabase,
                        profileDatabase,
                        notificationService,
						debugDatabase);
				}

				return Terminal;
			}
		}

		protected DebugTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IEventDatabase eventDatabase, IEtchingDatabase etchingDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            IProfileDatabase profileDatabase,
            INotificationService notificationService, IDebugDatabase debugDatabase)
			: base(logger,
					accountDatabase, adminDatabase, bannerDatabase,
					eventDatabase, etchingDatabase,
					disciplineDatabase, keyDatabase,
					mediaDatabase, notificationDatabase,
					profileDatabase,  notificationService)
		{
			DebugDatabase = debugDatabase;
			DebugDirector = new DebugDirector(this);
		}

		#endregion
	}
}