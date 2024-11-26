using Core;

namespace Frontier.Controllers
{
	public class GuardBox
	{
		public EnvironmentOptions env;
		public ILogger log;

		public IAccountOperations accounts;
		public IGatheringOperations gatherings;
		public ISnapshotOperations snapshots;
		public IKeyOperations keys;
		public IDisciplineOperations reports;
		public IMediaOperations media;
		public INotificationOperations telegrams;
		public INestOperations nests;
        public IMiscellaneousOperations miscellaneous;

        public GuardBox(EnvironmentOptions environment, ILogger logger,
			IAccountOperations accountOperations,
			INestOperations nestOperations, IGatheringOperations gatheringOperations,
			ISnapshotOperations snapshotOperations, IKeyOperations keyOperations,
			IDisciplineOperations disciplineOperations,IMediaOperations mediaOperations,
			INotificationOperations notificationOperations, IMiscellaneousOperations miscellaneousOperations)
		{
			env = environment;
			log = logger;

			accounts = accountOperations;
			nests = nestOperations;
			gatherings = gatheringOperations;
			snapshots = snapshotOperations;
			keys = keyOperations;
			reports = disciplineOperations;
			media = mediaOperations;
			telegrams = notificationOperations;
			miscellaneous = miscellaneousOperations;
		}
	}
}
