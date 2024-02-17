using System;
using System.Collections.Generic;
using System.Threading;
using Core.Controls;

namespace Core.Boundaries
{
	public class CoreTerminal
	{
		#region Variables

		public static CoreTerminal Terminal { get; private set; }
		private static object initLock = new();

		public IAccountDatabase AccountDatabase { get; init; }
		public IEventDatabase EventDatabase { get; init; }
		public IEtchingDatabase EtchingDatabase { get; init; }
		public IProfileDatabase ProfileDatabase { get; init; }
		public IDisciplineDatabase DisciplineDatabase { get; init; }
		public INotificationDatabase NotificationDatabase { get; init; }
		public IAdminDatabase AdminDatabase { get; init; }

		public IAccountOperations AccountOperations
			=> AccountDirector;
		public IEventOperations EventOperations
			=> EventDirector;
		public IEtchingOperations EtchingOperations
			=> EtchingDirector;
		public IProfileOperations ProfileOperations
			=> ProfileDirector;
		public IDisciplineOperations DisciplineOperations
			=> DisciplineDirector;
		public INotificationOperations NotificationOperations
			=> NotificationDirector;

		public INotificationService NotificationService { get; init; }

		internal AccountDirector AccountDirector { get; private set; }
		internal EventDirector EventDirector { get; private set; }
		internal EtchingDirector EtchingDirector { get; private set; }
		internal ProfileDirector ProfileDirector { get; private set; }
		internal DisciplineDirector DisciplineDirector { get; private set; }
		internal NotificationDirector NotificationDirector { get; private set; }

		public List<(Type GateType, object Instance)> Gates
			=> new() { (typeof(IAccountOperations), AccountOperations),
				(typeof(IEventOperations), EventOperations),
				(typeof(IEtchingOperations), EtchingOperations),
				(typeof(IProfileOperations), ProfileOperations),
				(typeof(IDisciplineOperations), DisciplineOperations),
				(typeof(INotificationOperations), NotificationOperations) };

		#endregion

		#region Initialisation

		public static CoreTerminal CreateTerminal(IAccountDatabase accountDatabase, IEventDatabase eventDatabase,
			IEtchingDatabase etchingDatabase, IProfileDatabase profileDatabase,
			IDisciplineDatabase disciplineDatabase, INotificationDatabase notificationDatabase,
			IAdminDatabase adminDatabase, INotificationService notificationService)
		{
			lock (initLock)
			{
				if (Terminal == null)
				{
					Terminal = new CoreTerminal(accountDatabase, eventDatabase, etchingDatabase, profileDatabase,
					disciplineDatabase, notificationDatabase, adminDatabase, notificationService);
				}
			
				return Terminal;
			}
		}

		private CoreTerminal(IAccountDatabase accountDatabase, IEventDatabase eventDatabase,
			IEtchingDatabase etchingDatabase, IProfileDatabase profileDatabase,
			IDisciplineDatabase disciplineDatabase, INotificationDatabase notificationDatabase,
			IAdminDatabase adminDatabase, INotificationService notificationService)
		{
			AccountDatabase = accountDatabase;
			EventDatabase = eventDatabase;
			EtchingDatabase = etchingDatabase;
			ProfileDatabase = profileDatabase;
			DisciplineDatabase = disciplineDatabase;
			NotificationDatabase = notificationDatabase;
			AdminDatabase = adminDatabase;

			NotificationService = notificationService;

			CreateManagers();
		}

		private void CreateManagers()
		{
			AccountDirector = new AccountDirector(this);
			EventDirector = new EventDirector(this);
			EtchingDirector = new EtchingDirector(this);
			ProfileDirector = new ProfileDirector(this);
			DisciplineDirector = new DisciplineDirector(this);
			NotificationDirector = new NotificationDirector(this);
		}

		#endregion
	}
}