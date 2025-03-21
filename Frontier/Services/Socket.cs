using System;
using System.Threading.Tasks;
using Core.Boundaries;
using Microsoft.AspNetCore.SignalR;

namespace Frontier.Services
{
	public class SocketService : ISocketService
    {
        private static ILogger log;
        private static IHubContext<MessageHub, IClientSocket> hub;

        public static void Initialise(ILogger logger, IHubContext<MessageHub, IClientSocket> hubContext)
        {
            log = logger;
            hub = hubContext;
        }

        public async Task BroadcastAsync()
        {
            await hub.Clients.All.ReceiveMessage();
        }
	}
}

