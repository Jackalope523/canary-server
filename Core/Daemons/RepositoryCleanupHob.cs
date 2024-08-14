using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Core.Daemons
{
	public class RepositoryCleanupService : BackgroundService
    {
        private readonly TimeSpan interval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan gatheringTimeout = TimeSpan.FromHours(1);

        public RepositoryCleanupService()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await KillZombieGatheringsAsync(stoppingToken);
                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task KillZombieGatheringsAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GuardBox>();
                var now = DateTime.UtcNow;

                var eventsToClose = await dbContext.Events
                    .Where(e => !e.IsClosed && e.StartedAt == null && now - e.CreatedAt >= _timeoutDuration)
                    .ToListAsync(stoppingToken);

                foreach (var evt in eventsToClose)
                {
                    evt.IsClosed = true;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}

