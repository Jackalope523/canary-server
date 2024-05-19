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
		public INotificationOperations notifications;
		public IProfileOperations profiles;

		public GuardBox(ILogger logger,
			IAccountOperations accountOperations, IBannerOperations bannerOperations,
			IProfileOperations profileOperations, IGatheringOperations gatheringOperations,
			ISnapshotOperations snapshotOperations, IKeyOperations keyOperations,
			IDisciplineOperations disciplineOperations,IMediaOperations mediaOperations,
			INotificationOperations notificationOperations)
		{
			log = logger;

			accounts = accountOperations;
			banners = bannerOperations;
			profiles = profileOperations;
			gatherings = gatheringOperations;
			snapshots = snapshotOperations;
			keys = keyOperations;
			reports = disciplineOperations;
			media = mediaOperations;
			notifications = notificationOperations;
		}
	}
}
