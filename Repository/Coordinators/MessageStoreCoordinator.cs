using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Repository
{
    class MessageStoreCoordinator : IMessageDatabase
    {
        private readonly IMessageDatabase store;

        public MessageStoreCoordinator(Harbor.Flag flag)
        {
            store = new EFMessageStore(flag);
        }

        public async Task<MessageShard> AddMessageAsync(long conversationId, long userId, DateTimeOffset timestamp, MessageType type, object value)
        {
            return await store.AddMessageAsync(conversationId, userId, timestamp, type, value);
        }

        public async Task AddUsersToConversationAsync(long conversationId, params long[] userIds)
        {
            await store.AddUsersToConversationAsync(conversationId, userIds);
        }

        public async Task<long> CreateGroupChatConversationAsync(string title = null)
        {
            return await store.CreateGroupChatConversationAsync(title);
        }

        public async Task DeleteConversationAsync(long conversationId)
        {
            await store.DeleteConversationAsync(conversationId);
        }

        public async Task<bool> GatheringConversationExists(long gatheringId)
        {
            return await store.GatheringConversationExists(gatheringId);
        }

        public async Task<CoreConversation> GetConversationAsync(long conversationId)
        {
            return await store.GetConversationAsync(conversationId);
        }

        public async Task<List<CoreMembership>> GetConversationMembersAsync(long conversationId)
        {
            return await store.GetConversationMembersAsync(conversationId);
        }

        public async Task<int> GetConversationPageCountAsync(long conversationId)
        {
            return await store.GetConversationPageCountAsync(conversationId);
        }

        public async Task<List<CoreConversation>> GetConversationsForUserAsync(long userId)
        {
            return await store.GetConversationsForUserAsync(userId);
        }

        public async Task<CoreMembership> GetMembershipAsync(long conversationId, long userId)
        {
            return await store.GetMembershipAsync(conversationId, userId);
        }

        public async Task<List<MessageShard>> GetMessagesForConversationAsync(long conversationId, int pageNumber)
        {
            return await store.GetMessagesForConversationAsync(conversationId, pageNumber);
        }

        public async Task<CoreConversation> GetOrCreateGatheringConversation(long gatheringId)
        {
            return await store.GetOrCreateGatheringConversation(gatheringId);
        }

        public async Task<CoreConversation> GetOrCreateIndividualConversationBetween(long userIdA, long userIdB)
        {
            return await store.GetOrCreateIndividualConversationBetween(userIdA, userIdB);
        }

        public async Task<bool> IndividualConversationBetweenExists(long userIdA, long userIdB)
        {
            return await store.IndividualConversationBetweenExists(userIdA, userIdB);
        }

        public async Task RemoveUserFromConversationAsync(long conversationId, long userId)
        {
            await store.RemoveUserFromConversationAsync(conversationId, userId);
        }

        public async Task UpdateConversationAsync(long conversationId, List<(string Property, object Value)> edits)
        {
            await store.UpdateConversationAsync(conversationId, edits);
        }

        public async Task UpdateMembershipAsync(long conversationId, long userId, List<(string Property, object Value)> edits)
        {
            await store.UpdateMembershipAsync(conversationId, userId, edits);
        }
    }
}
