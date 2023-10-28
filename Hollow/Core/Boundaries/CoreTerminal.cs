using System;
using System.Collections.Generic;
using Core.Controls;

namespace Core.Boundaries
{
	public class CoreTerminal
	{
		public static CoreTerminal Terminal { get; private set; }

		public IAccountDatabase AccountDatabase { get; init; }
		public IEventDatabase EventDatabase { get; init; }
		public IEtchingDatabase EtchingDatabase { get; init; }
		public IProfileDatabase ProfileDatabase { get; init; }
		public IReportDatabase ReportDatabase { get; init; }

		public IAccountOperations AccountOperations
			=> AccountDirector;
		public IEventOperations EventOperations
			=> EventDirector;
		public IEtchingOperations EtchingOperations
			=> EtchingDirector;
		public IProfileOperations ProfileOperations
			=> ProfileDirector;
		public IReportOperations ReportOperations
			=> ReportDirector;

		internal AccountDirector AccountDirector { get; private set; }
		internal EventDirector EventDirector { get; private set; }
		internal EtchingDirector EtchingDirector { get; private set; }
		internal ProfileDirector ProfileDirector { get; private set; }
		internal ReportDirector ReportDirector { get; private set; }

		public List<(Type DatabaseType, object Instance)> Gates
			=> new() { (typeof(IAccountOperations), AccountOperations),
				(typeof(IEventOperations), EventOperations),
				(typeof(IEtchingOperations), EtchingOperations),
				(typeof(IProfileOperations), ProfileOperations),
				(typeof(IReportOperations), ReportOperations) };

		public CoreTerminal(IAccountDatabase accountDatabase, IEventDatabase eventDatabase,
			IEtchingDatabase etchingDatabase, IProfileDatabase profileDatabase, IReportDatabase reportDatabase)
		{
			Terminal = this;

			AccountDatabase = accountDatabase;
			EventDatabase = eventDatabase;
			EtchingDatabase = etchingDatabase;
			ProfileDatabase = profileDatabase;
			ReportDatabase = reportDatabase;

			CreateManagers();

			BridgeManagers();
		}

		private void CreateManagers()
		{
			AccountDirector = new AccountDirector(this);
			EventDirector = new EventDirector(this);
			EtchingDirector = new EtchingDirector(this);
			ProfileDirector = new ProfileDirector(this);
			ReportDirector = new ReportDirector(this);
		}

		private void BridgeManagers()
		{
			AccountDirector.Bridge();
			EventDirector.Bridge();
			EtchingDirector.Bridge();
			ProfileDirector.Bridge();
			ReportDirector.Bridge();
		}
	}
}

