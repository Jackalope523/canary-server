using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;
using System.Collections.Concurrent;
using Core.Entities;


namespace Core.Controls
{
    internal class MessageDirector : AbstractDirector, IMessageOperations
    {
        #region Initialisation

        public MessageDirector(CoreTerminal terminal) : base(terminal) { }

        #endregion

        #region Operations

        public Task<List<ConversationShard>> GetConversationsAsync(long userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<MembershipShard>> GetConversationMembersAsync(long userId, long conversationId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<MessageShard>> GetMessagesAsync(long userId, long conversationId)
        {
            throw new System.NotImplementedException();
        }

        public Task UserReadAsync(long userId, long conversationId)
        {
            throw new System.NotImplementedException();
        }

        public Task UserComposingAsync(long userId, long conversationId)
        {
            throw new System.NotImplementedException();
        }

        public Task SendTextAsync(long userId, long conversationId, string text)
        {
            throw new System.NotImplementedException();
        }

        public Task SendPhotoAsync(long userId, long conversationId, MemoryStream photo)
        {
            throw new System.NotImplementedException();
        }

        public Task ShareGatheringAsync(long userId, long conversationId, long gatheringId)
        {
            throw new System.NotImplementedException();
        }

        public Task ShareSnapshotAsync(long userId, long conversationId, long snapshotId)
        {
            throw new System.NotImplementedException();
        }

        public Task ShareNestAsync(long userId, long conversationId, long nestId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConversationShard> CreateGroupChatAsync(long userId, params long[] participantIds)
        {
            throw new System.NotImplementedException();
        }

        public Task EditGroupChatAsync(long userId, long conversationId, string title = "")
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteGroupChatAsync(long userId, long conversationId)
        {
            throw new System.NotImplementedException();
        }

        public Task LeaveGroupChatAsync(long userId, long conversationId)
        {
            throw new System.NotImplementedException();
        }

        public Task SummonUserAsync(long userId, long conversationId, long targetId)
        {
            throw new System.NotImplementedException();
        }

        public Task KickUserAsync(long userId, long conversationId, long targetId)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Favours

        public bool IsUserOnline(User user) => UserConnections.ContainsKey(user.Id);

        #endregion
    }
}
