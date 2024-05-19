using System;
using System.Collections.Generic;
using System.Threading;
using Core.Boundaries;
using Core.Controls;
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
        public IEtchingDatabase EtchingDatabase { get; init; }
        public IDisciplineDatabase DisciplineDatabase { get; init; }
        public IKeyDatabase KeyDatabase { get; init; }
        public IMediaDatabase MediaDatabase { get; init; }
        public INotificationDatabase NotificationDatabase { get; init; }
        public IProfileDatabase ProfileDatabase { get; init; }

        public IAccountOperations AccountOperations
            => AccountDirector;
        public IBannerOperations BannerOperations
            => BannerDirector;
        public IGatheringOperations GatheringOperations
            => GatheringDirector;
        public IEtchingOperations EtchingOperations
            => EtchingDirector;
        public IDisciplineOperations DisciplineOperations
            => DisciplineDirector;
        public IKeyOperations KeyOperations
            => KeyDirector;
        public IMediaOperations MediaOperations
            => MediaDirector;
        public INotificationOperations NotificationOperations
            => NotificationDirector;
        public IProfileOperations ProfileOperations
            => ProfileDirector;

        public INotificationService NotificationService { get; init; }

        internal AccountDirector AccountDirector { get; private set; }
        internal BannerDirector BannerDirector { get; private set; }
        internal GatheringDirector GatheringDirector { get; private set; }
        internal EtchingDirector EtchingDirector { get; private set; }
        internal DisciplineDirector DisciplineDirector { get; private set; }
        internal KeyDirector KeyDirector { get; private set; }
        internal MediaDirector MediaDirector { get; private set; }
        internal NotificationDirector NotificationDirector { get; private set; }
        internal ProfileDirector ProfileDirector { get; private set; }

        #endregion

        #region Initialisation

        public static CoreTerminal CreateTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
            IGatheringDatabase gatheringDatabase, IEtchingDatabase etchingDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            IProfileDatabase profileDatabase,
            INotificationService notificationService)
        {
            lock (initLock)
            {
                Terminal ??= new CoreTerminal(logger,
                        accountDatabase, adminDatabase, bannerDatabase,
                        gatheringDatabase, etchingDatabase,
                        disciplineDatabase, keyDatabase,
                        mediaDatabase, notificationDatabase,
                        profileDatabase,
                        notificationService);

                return Terminal;
            }
        }

        protected CoreTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase, IBannerDatabase bannerDatabase,
			IGatheringDatabase gatheringDatabase, IEtchingDatabase etchingDatabase,
			IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            IProfileDatabase profileDatabase,
            INotificationService notificationService)
        {
            Log = logger;

            AccountDatabase = accountDatabase;
            AdminDatabase = adminDatabase;
            BannerDatabase = bannerDatabase;
            GatheringDatabase = gatheringDatabase;
            EtchingDatabase = etchingDatabase;
            DisciplineDatabase = disciplineDatabase;
            KeyDatabase = keyDatabase;
            MediaDatabase = mediaDatabase;
            NotificationDatabase = notificationDatabase;
            ProfileDatabase = profileDatabase;

            NotificationService = notificationService;

            CreateManagers();
        }

        private void CreateManagers()
        {
            AccountDirector = new AccountDirector(this);
            BannerDirector = new BannerDirector(this);
            GatheringDirector = new GatheringDirector(this);
            EtchingDirector = new EtchingDirector(this);
            DisciplineDirector = new DisciplineDirector(this);
            KeyDirector = new KeyDirector(this);
            MediaDirector = new MediaDirector(this);
            NotificationDirector = new NotificationDirector(this);
            ProfileDirector = new ProfileDirector(this);
        }

        #endregion
    }
}