using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.Extensions.Hosting;

using static Core.Entities.Psijic;

namespace Core.Daemons
{
	public class RepositoryCleanupGoblin : BackgroundService
    {
        private readonly TimeSpan interval = TimeSpan.FromMinutes(5);

        private CoreTerminal terminal;

        private ILogger log;

        public RepositoryCleanupGoblin(CoreTerminal coreTerminal)
        {
            terminal = coreTerminal;

            log = terminal.Log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                log.LogInformation("Gravekeeper goblin clocking in at {time}.", Time);

                try
                {
                    await KillZombieGatheringsAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    log.LogError("{goblin} had trouble: {error}", nameof(RepositoryCleanupGoblin), e);
                }

                log.LogInformation("Gravekeeper goblin clocking out at {time}.", Time);

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task KillZombieGatheringsAsync(CancellationToken stoppingToken)
        {
            var waitingGatherings = await terminal.AdminDatabase.GetAllWaitingGatheringsAsync(DateTimeOffset.UtcNow);

            foreach (var gathering in waitingGatherings)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                if (HasAlready(gathering.StartTime + Gathering.MaximumEarlyBirdStart))
                {
                    // Purge gathering
                    log.LogInformation("Gathering {id} {name} ended for being late.", gathering.Id, gathering.Name);
                    await terminal.AdminDatabase.VoidGatheringAsync(gathering.Id);

                    // Notify user
                    User host = new(gathering.Host);
                    await host.PostTelegram(User.Hollow, Boundaries.TelegramMessage.Generic,
                        $"Your gathering {gathering.Name} was removed for being late.");
                }
            }
        }
    }
}

