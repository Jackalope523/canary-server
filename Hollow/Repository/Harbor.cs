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
        public IPhotoDatabase PhotoDatabaseAccess { get; private set; }

        public Harbor(Flag flag)
        {
            AccountDatabaseAccess = new AccountStore(new EFCoreSentry(flag));
            ProfileDatabaseAccess = new ProfileStore(new EFCoreSentry(flag));
            NotificationDatabaseAccess = new NotificationStore(new EFCoreSentry(flag));
            EventDatabaseAccess = new EventStore(new EFCoreSentry(flag));
            EtchingDatabaseAccess = new EtchingStore(new EFCoreSentry(flag));
            ReportDatabaseAccess = new DisciplineStore(new EFCoreSentry(flag));
            AdminDatabaseAccess = new AdminStore(new EFCoreSentry(flag));
            PhotoDatabaseAccess = new PhotoStore(new AzureStorageSentry(flag));
        }
    }
}
