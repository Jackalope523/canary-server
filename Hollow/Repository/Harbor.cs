using Core.Boundaries;

namespace Repository
{
    public class Harbor
    {
        public enum Flag { Development, Production }

        public IAccountDatabase AccountDatabaseAccess { get; private set; }
        public IProfileDatabase ProfileDatabaseAccess { get; private set; }
        public INotificationDatabase NotificationDatabaseAccess { get; private set; }
        public IEventDatabase EventDatabaseAccess { get; private set; }
        public IEtchingDatabase EtchingDatabaseAccess { get; private set; }
        public IDisciplineDatabase ReportDatabaseAccess { get; private set; }
        public IAdminDatabase AdminDatabaseAccess { get; private set; }
        public IImageDatabase PhotoDatabaseAccess { get; private set; }

        public Harbor(Flag flag)
        {
            AccountDatabaseAccess = new AccountStoreCoordinator(flag);
            ProfileDatabaseAccess = new ProfileStoreCoordinator(flag);
            NotificationDatabaseAccess = new NotificationStoreCoordinator(flag);
            EventDatabaseAccess = new EventStoreCoordinator(flag);
            EtchingDatabaseAccess = new EtchingStoreCoordinator(flag);
            ReportDatabaseAccess = new DisciplineStoreCoordinator(flag);
            AdminDatabaseAccess = new AdminStoreCoordinator(flag);
            PhotoDatabaseAccess = new MediaStoreCoordinator();
        }
    }
}
