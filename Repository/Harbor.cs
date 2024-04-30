using Core.Boundaries;
using Microsoft.Extensions.Logging;
using Repository.Coordinators;

namespace Repository
{
    public class Harbor
    {
        public enum Flag { Development, Production }

        internal static ILogger logger;

        public IAccountDatabase AccountDatabaseAccess { get; private set; }
        public IProfileDatabase ProfileDatabaseAccess { get; private set; }
        public INotificationDatabase NotificationDatabaseAccess { get; private set; }
        public IEventDatabase EventDatabaseAccess { get; private set; }
        public IEtchingDatabase EtchingDatabaseAccess { get; private set; }
        public IDisciplineDatabase ReportDatabaseAccess { get; private set; }
        public IAdminDatabase AdminDatabaseAccess { get; private set; }
        public IMediaDatabase MediaDatabaseAccess { get; private set; }
        public IKeyDatabase KeyDatabaseAccess { get; private set; }
        public IBannerDatabase BannerDatabaseAccess { get; private set; }
        public IDebugDatabase DebugDatabaseAccess { get; private set; }

        public Harbor(Flag flag)
        {
            AccountDatabaseAccess = new AccountStoreCoordinator(flag);
            ProfileDatabaseAccess = new ProfileStoreCoordinator(flag);
            NotificationDatabaseAccess = new NotificationStoreCoordinator(flag);
            EventDatabaseAccess = new EventStoreCoordinator(flag);
            EtchingDatabaseAccess = new EtchingStoreCoordinator(flag);
            ReportDatabaseAccess = new DisciplineStoreCoordinator(flag);
            AdminDatabaseAccess = new AdminStoreCoordinator(flag);
            MediaDatabaseAccess = new MediaStoreCoordinator();
            KeyDatabaseAccess = new KeyStoreCoordinator();
            DebugDatabaseAccess = new DebugStoreCoordinator(flag);
        }

        public Harbor(Flag flag, ILogger logger) : this(flag)
        {
            Harbor.logger = logger;
        }
    }
}
