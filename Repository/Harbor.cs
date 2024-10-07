using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Coordinators;

namespace Repository
{
    public class Harbor
    {
        public enum Flag { Development, Production }

        internal static ILogger logger;

        public IAccountDatabase AccountDatabaseAccess { get; private set; }
        public INestDatabase NestDatabaseAccess { get; private set; }
        public INotificationDatabase NotificationDatabaseAccess { get; private set; }
        public IGatheringDatabase GatheringDatabaseAccess { get; private set; }
        public ISnapshotDatabase SnapshotDatabaseAccess { get; private set; }
        public IDisciplineDatabase ReportDatabaseAccess { get; private set; }
        public IAdminDatabase AdminDatabaseAccess { get; private set; }
        public IMediaDatabase MediaDatabaseAccess { get; private set; }
        public IKeyDatabase KeyDatabaseAccess { get; private set; }
        public IBannerDatabase BannerDatabaseAccess { get; private set; }
        public IDebugDatabase DebugDatabaseAccess { get; private set; }
        public IMiscellaneousDatabase MiscellaneousDatabaseAccess { get; private set; }

        public Harbor(Flag flag)
        {
            AccountDatabaseAccess = new AccountStoreCoordinator(flag);
            NestDatabaseAccess = new NestStoreCoordinator(flag);
            NotificationDatabaseAccess = new NotificationStoreCoordinator(flag);
            GatheringDatabaseAccess = new GatheringStoreCoordinator(flag);
            SnapshotDatabaseAccess = new SnapshotStoreCoordinator(flag);
            ReportDatabaseAccess = new DisciplineStoreCoordinator(flag);
            AdminDatabaseAccess = new AdminStoreCoordinator(flag);
            BannerDatabaseAccess = new BannerStoreCoordinator(flag);
            KeyDatabaseAccess = new KeyStoreCoordinator();
            MediaDatabaseAccess = new MediaStoreCoordinator();
            DebugDatabaseAccess = new DebugStoreCoordinator(flag);
            MiscellaneousDatabaseAccess = new MiscellaneousStoreCoordinator(flag);
        }

        public Harbor(Flag flag, ILogger logger) : this(flag)
        {
            Harbor.logger = logger;
        }
    }
}
