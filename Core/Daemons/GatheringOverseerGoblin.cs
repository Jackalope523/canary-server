using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Core.Notifications;
using Microsoft.Extensions.Hosting;

using static Core.Entities.Psijic;

namespace Core.Daemons
{
	public class GatheringOverseerGoblin : BackgroundService
    {
        private readonly TimeSpan interval = TimeSpan.FromMinutes(5);

        private CoreTerminal terminal;

        private ILogger log;

        public GatheringOverseerGoblin(CoreTerminal coreTerminal)
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
                    log.LogError("{goblin} had trouble: {error}", nameof(GatheringOverseerGoblin), e);
                }

                log.LogInformation("Gravekeeper goblin clocking out at {time}.", Time);

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task KillZombieGatheringsAsync(CancellationToken stoppingToken)
        {
            var waitingGatherings = await terminal.AdminDatabase.GetAllWaitingGatheringsAsync(DateTimeOffset.UtcNow);

            foreach (var coreGathering in waitingGatherings)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                Gathering gathering = new(coreGathering);

                // Check if gathering has expired
                if (HasAlready(gathering.StartTime + Gathering.MaximumStartWait))
                {
                    // Purge gathering
                    log.LogInformation("Gathering {id} {name} cancelled for being late.", gathering.Id, gathering.Title);
                    await terminal.GatheringDatabase.CancelGatheringAsync(gathering.Id);

                    // Notify host
                    User host = await GetUserAsync(gathering.Host.Id);
                    await host.PostTelegram(User.Hollow, TelegramMessage.GatheringMissedHost, $"{gathering.Title}");
                    await host.Notify(CanaryNotification.GatheringDeleted(gathering.ToGatheringShard()));

                    // Notify guests
                    await gathering.NotifyGuests(CanaryNotification.HostMissedGathering(gathering.ToGatheringShard()));
                }
                // Check if the next pass will delete the gathering
                else if (HasAlready(gathering.StartTime + Gathering.MaximumStartWait - interval))
                {
                    // Warn host
                    User host = await GetUserAsync(gathering.Host.Id);
                    await host.Notify(CanaryNotification.GatheringRemovalWarning(gathering.ToGatheringShard()));
                }
            }
        }

        private async Task<User> GetUserAsync(long userId)
        {
            return new(await terminal.AccountDatabase.FindUserByIdAsync(userId));
        }
    }
}

