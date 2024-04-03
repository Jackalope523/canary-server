using Core.Boundaries;
using Shared;

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
        public IMediaDatabase PhotoDatabaseAccess { get; private set; }

        public Harbor(Flag flag)
        {
            AccountDatabaseAccess = new EFCoreAccountStore(flag);
            ProfileDatabaseAccess = new EFCoreProfileStore(flag);
            NotificationDatabaseAccess = new EFCoreNotificationStore(flag);
            EventDatabaseAccess = new EFCoreEventStore(flag);
            EtchingDatabaseAccess = new EFCoreEtchingStore(flag);
            ReportDatabaseAccess = new EFCoreDisciplineStore(flag);
            AdminDatabaseAccess = new EFCoreAdminStore(flag);
            PhotoDatabaseAccess = new AzureImageStore();
        }
    }
}
