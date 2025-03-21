using Frontier.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Frontier
{
    public class MessageHub : Hub<IClientSocket>
    {
        public IMessageOperations messages;
        public UserManager<CoreUser> userManager;

        public MessageHub(GuardBox box, UserManager<CoreUser> aspUserManager)
        {
            messages = box.messages;
            userManager = aspUserManager;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var user = await GetCurrentUserAsync();
            var connectionId = Context.ConnectionId;

            await messages.UserConnectedAsync(user.Id, connectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            var user = await GetCurrentUserAsync();
            var connectionId = Context.ConnectionId;

            await messages.UserDisconnectedAsync(user.Id, connectionId);
        }

        // Methods

        public async Task UserRead(long conversationId)
        {
            var user = await GetCurrentUserAsync();

            await messages.UserReadAsync(user.Id, conversationId);
        }

        public async Task UserComposing(long conversationId)
        {
            var user = await GetCurrentUserAsync();

            await messages.UserComposingAsync(user.Id, conversationId);
        }

        public async Task SendText(long conversationId, string text)
        {
            var user = await GetCurrentUserAsync();

            await messages.SendTextAsync(user.Id, conversationId, text);
        }

        public async Task SendPhoto(long conversationId, MemoryStream photo)
        {
            var user = await GetCurrentUserAsync();

            await messages.SendPhotoAsync(user.Id, conversationId, photo);
        }

        public async Task ShareGathering(long conversationId, long gatheringId)
        {
            var user = await GetCurrentUserAsync();

            await messages.ShareGatheringAsync(user.Id, conversationId, gatheringId);
        }

        public async Task ShareSnapshot(long conversationId, long snapshotId)
        {
            var user = await GetCurrentUserAsync();

            await messages.ShareSnapshotAsync(user.Id, conversationId, snapshotId);
        }

        public async Task ShareNest(long conversationId, long nestId)
        {
            var user = await GetCurrentUserAsync();

            await messages.ShareNestAsync(user.Id, conversationId, nestId);
        }

        private async Task<CoreUser> GetCurrentUserAsync()
            => await userManager.GetUserAsync(Context.User);
    }
}
