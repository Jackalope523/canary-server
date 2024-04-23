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
		public IEventOperations events;
		public IEtchingOperations etchings;
		public IKeyOperations keys;
		public IDisciplineOperations reports;
		public IMediaOperations media;
		public INotificationOperations notifications;
		public IProfileOperations profiles;

		public GuardBox(ILogger logger,
			IAccountOperations accountOperations, IBannerOperations bannerOperations,
			IProfileOperations profileOperations, IEventOperations eventOperations,
			IEtchingOperations etchingOperations, IKeyOperations keyOperations,
			IDisciplineOperations disciplineOperations,IMediaOperations mediaOperations,
			INotificationOperations notificationOperations)
		{
			log = logger;

			accounts = accountOperations;
			banners = bannerOperations;
			profiles = profileOperations;
			events = eventOperations;
			etchings = etchingOperations;
			keys = keyOperations;
			reports = disciplineOperations;
			media = mediaOperations;
			notifications = notificationOperations;
		}
	}
}
