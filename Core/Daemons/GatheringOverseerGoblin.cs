using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
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

            foreach (var gathering in waitingGatherings)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                // Check if gathering has expired
                if (HasAlready(gathering.StartTime + Gathering.MaximumStartWait))
                {
                    // Purge gathering
                    log.LogInformation("Gathering {id} {name} ended for being late.", gathering.Id, gathering.Title);
                    await terminal.AdminDatabase.VoidGatheringAsync(gathering.Id);

                    // Notify host
                    User host = await GetUserAsync(gathering.Host.Id);
                    await host.PostTelegram(User.Hollow, TelegramMessage.GatheringMissedHost, $"{gathering.Title}");
                }
                // Check if the next pass will delete the gathering
                else if (HasAlready(gathering.StartTime + Gathering.MaximumStartWait - interval))
                {
                    // Warn host
                    User host = await GetUserAsync(gathering.Host.Id);
                    await host.Notify(NotificationGroup.GatheringActivity, "Your gathering is about to be deleted.",
                        $"{gathering.Title} is going to be deleted if you do not start it!", "30");
                }
            }
        }

        private async Task<User> GetUserAsync(long userId)
        {
            return new(await terminal.AccountDatabase.FindUserByIdAsync(userId));
        }
    }
}

