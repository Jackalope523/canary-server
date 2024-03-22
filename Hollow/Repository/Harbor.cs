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

        public Harbor(Flag flag)
        {
            switch (flag)
            {
                case Flag.Development:
                    AccountDatabaseAccess = new AccountStore(new TestSentry());
                    ProfileDatabaseAccess = new ProfileStore(new TestSentry());
                    NotificationDatabaseAccess = new NotificationStore(new TestSentry());
                    EventDatabaseAccess = new EventStore(new TestSentry());
                    EtchingDatabaseAccess = new EtchingStore(new TestSentry());
                    ReportDatabaseAccess = new DisciplineStore(new TestSentry());
                    AdminDatabaseAccess = new AdminStore(new TestSentry());
                    break;

                case Flag.Production:
                    AzureSentry.SeedDatabase();
                    AccountDatabaseAccess = new AccountStore(new AzureSentry());
                    ProfileDatabaseAccess = new ProfileStore(new AzureSentry());
                    NotificationDatabaseAccess = new NotificationStore(new AzureSentry());
                    EventDatabaseAccess = new EventStore(new AzureSentry());
                    EtchingDatabaseAccess = new EtchingStore(new AzureSentry());
                    ReportDatabaseAccess = new DisciplineStore(new AzureSentry());
                    AdminDatabaseAccess = new AdminStore(new AzureSentry());
                    break;

                default:
                    throw new UndefinedHarborStateException();
            }
        }
    }
}
