using System;
using System.Collections.Generic;
using Server.Controls;

namespace Server.Boundaries
{
	public class CoreTerminal
	{
		public IAccountDatabase AccountDatabase { get; init; }
		public IEventDatabase EventDatabase { get; init; }
		public IPostDatabase PostDatabase { get; init; }
		public IProfileDatabase ProfileDatabase { get; init; }
		public IReportDatabase ReportDatabase { get; init; }

		public IAccountOperations AccountOperations
			=> AccountManager;
		public IEventOperations EventOperations
			=> EventManager;
		public IPostOperations PostOperations
			=> PostManager;
		public IProfileOperations ProfileOperations
			=> ProfileManager;
		public IReportOperations ReportOperations
			=> ReportManager;

		internal AccountManager AccountManager { get; private set; }
		internal EventManager EventManager { get; private set; }
		internal PostManager PostManager { get; private set; }
		internal ProfileManager ProfileManager { get; private set; }
		internal ReportManager ReportManager { get; private set; }

		public List<(Type DatabaseType, object Instance)> Gates
			=> new() { (typeof(IAccountOperations), AccountOperations),
				(typeof(IEventDatabase), EventOperations),
				(typeof(IPostOperations), PostOperations),
				(typeof(IProfileOperations), ProfileOperations),
				(typeof(IReportOperations), ReportOperations) };

		public CoreTerminal(IAccountDatabase accountDatabase, IEventDatabase eventDatabase,
			IPostDatabase postDatabase, IProfileDatabase profileDatabase, IReportDatabase reportDatabase)
		{
			AccountDatabase = accountDatabase;
			EventDatabase = eventDatabase;
			PostDatabase = postDatabase;
			ProfileDatabase = profileDatabase;
			ReportDatabase = reportDatabase;

			CreateManagers();

			BridgeManagers();
		}

		private void CreateManagers()
		{
			AccountManager = new AccountManager(this);
			EventManager = new EventManager(this);
			PostManager = new PostManager(this);
			ProfileManager = new ProfileManager(this);
			ReportManager = new ReportManager(this);
		}

		private void BridgeManagers()
		{
			AccountManager.Bridge();
			EventManager.Bridge();
			PostManager.Bridge();
			ProfileManager.Bridge();
			ReportManager.Bridge();
		}
	}
}

