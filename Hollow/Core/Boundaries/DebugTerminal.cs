using System;
using System.Collections.Generic;
using System.Threading;
using Core.Controls;

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

		public static CoreTerminal CreateDebugTerminal(IAccountDatabase accountDatabase, IEventDatabase eventDatabase,
			IEtchingDatabase etchingDatabase, IProfileDatabase profileDatabase,
			IDisciplineDatabase disciplineDatabase, INotificationDatabase notificationDatabase,
			IAdminDatabase adminDatabase, INotificationService notificationService, IDebugDatabase debugDatabase)
		{
			lock (initLock)
			{
				if (Terminal == null)
				{
					Terminal = new DebugTerminal(accountDatabase, eventDatabase, etchingDatabase, profileDatabase,
					disciplineDatabase, notificationDatabase, adminDatabase, notificationService, debugDatabase);
				}

				return Terminal;
			}
		}

		protected DebugTerminal(IAccountDatabase accountDatabase, IEventDatabase eventDatabase,
			IEtchingDatabase etchingDatabase, IProfileDatabase profileDatabase,
			IDisciplineDatabase disciplineDatabase, INotificationDatabase notificationDatabase,
			IAdminDatabase adminDatabase, INotificationService notificationService, IDebugDatabase debugDatabase)
			: base(accountDatabase, eventDatabase, etchingDatabase, profileDatabase,
				disciplineDatabase, notificationDatabase, adminDatabase, notificationService)
		{
			DebugDatabase = debugDatabase;

			CreateManagers();
		}

		private void CreateManagers()
		{
			DebugDirector = new DebugDirector(this);
		}

		#endregion
	}
}