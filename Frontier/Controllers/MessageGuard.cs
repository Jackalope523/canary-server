using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontier
{
    public class MessageGuard : Hub
    {
        // ADDING AND REMOVING USERS TO GROUPS
        // CATCH UP MESSAGES
        public async Task SendMessageToGroupAsync(string groupId, string user, string message)
        {
            await Clients.GroupExcept(groupId, Context.ConnectionId).SendAsync("ReceiveMessage", user, message);

            // WriteMessageAndViews(long groupId, long userId, string message, List<long> connectedUsers)
            // WRITE MESSAGE TO DATABASE
            // GET GROUP MEMBERS FROM DATABASE
        }

        public async Task AddUsersToGroupAsync(string groupId, List<long> users)
        {
            // CALLED ON CHAT CREATION
            // ADD CONNECTED USERS TO THE CHAT
        }

        public async Task RemoveUsersFromGroupAsync(string groupId, List<long> users)
        {
            // CALLED ON CHAT DELETION
            // REMOVE CONNECTED USERS FROM A DELETED CHAT
        }

        public override async Task OnConnectedAsync()
        {
            // GET GROUP IDS THIS USER IS A PART OF.
            List<long> groupIds = new();

            foreach (long id in groupIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // GET GROUP IDS THIS USER IS A PART OF.
            List<long> groupIds = new();

            foreach (long id in groupIds)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
