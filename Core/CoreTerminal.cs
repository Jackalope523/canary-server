using System;
using Core.Boundaries;
using Core.Controls;
using Core.Daemons;

namespace Core
{
    public enum EnvironmentFlag
    {
        Production, Staging, Development
    }

    public record EnvironmentOptions
    {
        public EnvironmentFlag Flag { get; init; }

        public bool IsProduction => Flag.Equals(EnvironmentFlag.Production);
    }

    public class CoreTerminal
    {
        #region Variables

        public static CoreTerminal Terminal { get; protected set; }
        private static object initLock = new();

        public EnvironmentOptions Environment { get; init; }

        public ILogger Log { get; init; }

        public IAccountDatabase AccountDatabase { get; init; }
        public IAdminDatabase AdminDatabase { get; init; }
        public IBannerDatabase BannerDatabase { get; init; }
        public IGatheringDatabase GatheringDatabase { get; init; }
        public ISnapshotDatabase SnapshotDatabase { get; init; }
        public IDisciplineDatabase DisciplineDatabase { get; init; }
        public IKeyDatabase KeyDatabase { get; init; }
        public IMediaDatabase MediaDatabase { get; init; }
        public INotificationDatabase NotificationDatabase { get; init; }
        public INestDatabase NestDatabase { get; init; }
        public IMiscellaneousDatabase MiscellaneousDatabase { get; init; }

        public IAccountOperations AccountOperations
            => AccountDirector;
        public IBannerOperations BannerOperations
            => BannerDirector;
        public IGatheringOperations GatheringOperations
            => GatheringDirector;
        public ISnapshotOperations SnapshotOperations
            => SnapshotDirector;
        public IDisciplineOperations DisciplineOperations
            => DisciplineDirector;
        public IKeyOperations KeyOperations
            => KeyDirector;
        public IMediaOperations MediaOperations
            => MediaDirector;
        public INotificationOperations NotificationOperations
            => NotificationDirector;
        public INestOperations NestOperations
            => NestDirector;
        public IMiscellaneousOperations MiscellaneousOperations
            => MiscellaneousDirector;

        public INotificationService NotificationService { get; init; }

        internal AccountDirector AccountDirector { get; private set; }
        internal BannerDirector BannerDirector { get; private set; }
        internal GatheringDirector GatheringDirector { get; private set; }
        internal SnapshotDirector SnapshotDirector { get; private set; }
        internal DisciplineDirector DisciplineDirector { get; private set; }
        internal KeyDirector KeyDirector { get; private set; }
        internal MediaDirector MediaDirector { get; private set; }
        internal NotificationDirector NotificationDirector { get; private set; }
        internal NestDirector NestDirector { get; private set; }
        internal MiscellaneousDirector MiscellaneousDirector { get; private set; }

        #endregion

        #region Initialisation

        public static CoreTerminal CreateTerminal(EnvironmentOptions environment, ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IGatheringDatabase gatheringDatabase, ISnapshotDatabase snapshotDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            INestDatabase nestDatabase, IMiscellaneousDatabase miscellaneousDatabase,
            INotificationService notificationService)
        {
            lock (initLock)
            {
                Terminal ??= new CoreTerminal()
                {
                    Environment = environment,
                    Log = logger,

                    AccountDatabase = accountDatabase,
                    AdminDatabase = adminDatabase,
                    BannerDatabase = bannerDatabase,
                    GatheringDatabase = gatheringDatabase,
                    SnapshotDatabase = snapshotDatabase,
                    DisciplineDatabase = disciplineDatabase,
                    KeyDatabase = keyDatabase,
                    MediaDatabase = mediaDatabase,
                    NotificationDatabase = notificationDatabase,
                    NestDatabase = nestDatabase,
                    MiscellaneousDatabase = miscellaneousDatabase,

                    NotificationService = notificationService,
                };

                Terminal.CreateManagers();

                return Terminal;
            }
        }

        protected CoreTerminal()
        { }

        protected void CreateManagers()
        {
            AccountDirector = new AccountDirector(this);
            BannerDirector = new BannerDirector(this);
            GatheringDirector = new GatheringDirector(this);
            SnapshotDirector = new SnapshotDirector(this);
            DisciplineDirector = new DisciplineDirector(this);
            KeyDirector = new KeyDirector(this);
            MediaDirector = new MediaDirector(this);
            NotificationDirector = new NotificationDirector(this);
            NestDirector = new NestDirector(this);
            MiscellaneousDirector = new MiscellaneousDirector(this);
        }

        #endregion

        #region Daemons

        public GatheringOverseerGoblin CreateRepositoryCleanupService()
         => new(this);


        public TelegramCleanupGoblin CreateTelegramCleanupService()
         => new(this);

        #endregion
    }
}