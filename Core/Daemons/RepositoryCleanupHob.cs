using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;

namespace Core.Daemons
{
	public class RepositoryCleanupService : BackgroundService
    {
        private readonly TimeSpan interval = TimeSpan.FromMinutes(5);

        private CoreTerminal terminal;

        private ILogger log;

        public RepositoryCleanupService(CoreTerminal coreTerminal)
        {
            terminal = coreTerminal;

            log = terminal.Log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await KillZombieGatheringsAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    log.LogError("Hob had trouble: {error}", e);
                }

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task KillZombieGatheringsAsync(CancellationToken stoppingToken)
        {
            log.LogInformation("Cleaning up repository.");

            var waitingGatherings = await terminal.AdminDatabase.GetAllWaitingGatherings(DateTimeOffset.UtcNow);

            foreach (var gathering in waitingGatherings)
            {
                if (!IsWithin(Time - gathering.StartTime, Gathering.MaximumEarlyBirdStart))
                {
                    log.LogInformation("Gathering {id} {name} ended for being late.", gathering.Id, gathering.Name);
                    await terminal.GatheringDatabase.EndGatheringAsync(gathering.Id, Time);
                }
            }
        }
    }
}

