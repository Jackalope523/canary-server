using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using Microsoft.Extensions.Hosting;

using static Core.Entities.Psijic;

namespace Core.Daemons
{
	public class TelegramCleanupGoblin : BackgroundService
    {
        private readonly TimeSpan interval = TimeSpan.FromHours(1);

        private CoreTerminal terminal;

        private ILogger log;

        public TelegramCleanupGoblin(CoreTerminal coreTerminal)
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
                    log.LogInformation("Telegram goblin clocking in at {time}.", Time);
                    await ReclaimExpiredTelegramsAsync(stoppingToken);
                    log.LogInformation("Telegram goblin clocking out at {time}.", Time);
                }
                catch (Exception e)
                {
                    log.LogError("{goblin} had trouble: {error}", nameof(TelegramCleanupGoblin), e);
                }

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task ReclaimExpiredTelegramsAsync(CancellationToken stoppingToken)
        {
            var telegrams = await terminal.NotificationDatabase.GetAllTelegramsAsync(TelegramMessage.GatheringInvitation);

            foreach (var telegram in telegrams)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    ulong gatheringId = ulong.Parse(telegram.Context);
                    var gathering = await terminal.GatheringDatabase.FindGatheringAsync(gatheringId);

                    // Check if gathering has ended
                    if (gathering.State.Equals(GatheringState.Ended))
                    {
                        log.LogInformation("Telegram {id} expired, deleting.", telegram.Id);
                        await terminal.NotificationDatabase.DeleteTelegramAsync(telegram.Id);
                    }
                }
                catch (HollowException e)
                {
                    log.LogInformation("Hollow exception trying to check telegram {id}. {error}", telegram.Id, e);
                }
                catch (Exception e)
                {
                    log.LogInformation("Clearing telegram {id} for having corrupted context. {error}", telegram.Id, e);
                    await terminal.NotificationDatabase.DeleteTelegramAsync(telegram.Id);
                }
            }
        }
    }
}

