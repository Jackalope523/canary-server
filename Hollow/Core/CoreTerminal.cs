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

        public static CoreTerminal Terminal { get; private set; }
        private static object initLock = new();

        public ILogger Log { get; init; }

        public IAccountDatabase AccountDatabase { get; init; }
        public IAdminDatabase AdminDatabase { get; init; }
        public IEventDatabase EventDatabase { get; init; }
        public IEtchingDatabase EtchingDatabase { get; init; }
        public IDisciplineDatabase DisciplineDatabase { get; init; }
        public IKeyDatabase KeyDatabase { get; init; }
        public IMediaDatabase MediaDatabase { get; init; }
        public INotificationDatabase NotificationDatabase { get; init; }
        public IProfileDatabase ProfileDatabase { get; init; }

        public IAccountOperations AccountOperations
            => AccountDirector;
        public IEventOperations EventOperations
            => EventDirector;
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
        internal EventDirector EventDirector { get; private set; }
        internal EtchingDirector EtchingDirector { get; private set; }
        internal DisciplineDirector DisciplineDirector { get; private set; }
        internal KeyDirector KeyDirector { get; private set; }
        internal MediaDirector MediaDirector { get; private set; }
        internal NotificationDirector NotificationDirector { get; private set; }
        internal ProfileDirector ProfileDirector { get; private set; }

        public List<(Type GateType, object Instance)> Gates
            => new() { (typeof(IAccountOperations), AccountOperations),
                (typeof(IEventOperations), EventOperations),
                (typeof(IEtchingOperations), EtchingOperations),
                (typeof(IDisciplineOperations), DisciplineOperations),
                (typeof(IKeyOperations), KeyOperations),
                (typeof(IMediaOperations), MediaOperations),
                (typeof(INotificationOperations), NotificationOperations),
                (typeof(IProfileOperations), ProfileOperations) };

        #endregion

        #region Initialisation

        public static CoreTerminal CreateTerminal(ILogger logger,
            IAccountDatabase accountDatabase, IAdminDatabase adminDatabase,
            IEventDatabase eventDatabase, IEtchingDatabase etchingDatabase,
            IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            IProfileDatabase profileDatabase,
            INotificationService notificationService)
        {
            lock (initLock)
            {
                Terminal ??= new CoreTerminal(logger,
                        accountDatabase, adminDatabase,
                        eventDatabase, etchingDatabase,
                        disciplineDatabase, keyDatabase,
                        mediaDatabase, notificationDatabase,
                        profileDatabase,
                        notificationService);

                return Terminal;
            }
        }

        private CoreTerminal(ILogger logger, IAccountDatabase accountDatabase, IAdminDatabase adminDatabase,
			IEventDatabase eventDatabase, IEtchingDatabase etchingDatabase,
			IDisciplineDatabase disciplineDatabase, IKeyDatabase keyDatabase,
            IMediaDatabase mediaDatabase, INotificationDatabase notificationDatabase,
            IProfileDatabase profileDatabase,
            INotificationService notificationService)
        {
            Log = logger;

            AccountDatabase = accountDatabase;
            AdminDatabase = adminDatabase;
            EventDatabase = eventDatabase;
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
            EventDirector = new EventDirector(this);
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