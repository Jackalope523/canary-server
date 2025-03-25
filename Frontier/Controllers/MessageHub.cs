using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Frontier.Controllers
{
    public partial class HollowHub : Hub<IClientSocket>
    {
        public async Task UserRead(long conversationId)
        {
            var user = await GetCurrentUserAsync();

            await messages.UserReadAsync(user.Id, conversationId);
        }

        public async Task UserComposing(long conversationId, bool isComposing)
        {
            var user = await GetCurrentUserAsync();

            await messages.UserComposingAsync(user.Id, conversationId, isComposing);
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
    }
}
