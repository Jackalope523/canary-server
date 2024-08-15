using System;
using System.Collections.Generic;
using System.Threading;
using Core.Boundaries;
using Core.Controls;
using Core.Daemons;
using Microsoft.Extensions.Logging;

namespace Core
{
    public class CoreTerminal
    {
        #region Variables

        public static CoreTerminal Terminal { get; protected set; }
        private static object initLock = new();

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

        #endregion

        #region Initialisation

        public static CoreTerminal CreateTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IGatheringDatabase gatheringDatabase, ISnapshotDatabase snapshotDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            INestDatabase nestDatabase,
            INotificationService notificationService)
        {
            lock (initLock)
            {
                Terminal ??= new CoreTerminal()
                {
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

                    NotificationService = notificationService,
                };

                return Terminal;
            }
        }

        protected CoreTerminal()
        {
            CreateManagers();
        }

        private void CreateManagers()
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
        }

        #endregion

        #region

        public RepositoryCleanupService CreateRepositoryCleanupService()
        {
            return new(this);
        }

        #endregion
    }
}