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
        private readonly TimeSpan interval = TimeSpan.FromMinutes(15);

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
                    await CorrodeGatheringsAsync(stoppingToken);
                }
                catch (Exception e)
                {
                    log.LogError("{goblin} had trouble: {error}", nameof(GatheringOverseerGoblin), e);
                }

                log.LogInformation("Gravekeeper goblin clocking out at {time}.", Time);

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task CorrodeGatheringsAsync(CancellationToken stoppingToken)
        {
            var waitingGatherings = await terminal.AdminDatabase.GetAllActiveGatheringsAsync(Psijic.Time);

            foreach (var coreGathering in waitingGatherings)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                Gathering gathering = new(coreGathering);

                float frequency = interval.Minutes / 60;
                int durationMinutes = gathering.Duration.Minutes;

                int newDecay = (int) (gathering.Decay - 50 * frequency);

                // If gathering has expired
                if (newDecay <= 0)
                {
                    // Terminate gathering
                    await terminal.GatheringDirector.TerminateGatheringAsync(gathering.HostId, gathering.Id);
                }
                else
                {
                    await terminal.GatheringDatabase.UpdateGatheringAsync(gathering.Id, new() { (nameof(CoreGathering.Decay), newDecay) });
                }
            }
        }

        private async Task<User> GetUserAsync(long userId)
        {
            return new(await terminal.AccountDatabase.FindUserByIdAsync(userId));
        }
    }
}

