using Core.Boundaries;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
	public class GuardBox
	{
		public ILogger log;

		public IAccountOperations accounts;
		public IBannerOperations banners;
		public IGatheringOperations gatherings;
		public ISnapshotOperations snapshots;
		public IKeyOperations keys;
		public IDisciplineOperations reports;
		public IMediaOperations media;
		public INotificationOperations telegrams;
		public INestOperations nests;
        public IMiscellaneousOperations miscellaneous;

        public GuardBox(ILogger logger,
			IAccountOperations accountOperations, IBannerOperations bannerOperations,
			INestOperations nestOperations, IGatheringOperations gatheringOperations,
			ISnapshotOperations snapshotOperations, IKeyOperations keyOperations,
			IDisciplineOperations disciplineOperations,IMediaOperations mediaOperations,
			INotificationOperations notificationOperations, IMiscellaneousOperations miscellaneousOperations)
		{
			log = logger;

			accounts = accountOperations;
			banners = bannerOperations;
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
