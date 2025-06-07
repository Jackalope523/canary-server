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

        public Task<MessageShard> AddMessageAsync(long conversationId, long userId, DateTimeOffset timestamp, MessageType type, object value)
        {
            return store.AddMessageAsync(conversationId, userId, timestamp, type, value);
        }

        public Task AddUsersToConversationAsync(long conversationId, params long[] userIds)
        {
            return store.AddUsersToConversationAsync(conversationId, userIds);
        }

        public Task<long> CreateGroupChatConversationAsync(string title = null)
        {
            return store.CreateGroupChatConversationAsync(title);
        }

        public Task DeleteConversationAsync(long conversationId)
        {
            return store.DeleteConversationAsync(conversationId);
        }

        public Task<bool> GatheringConversationExists(long gatheringId)
        {
            return store.GatheringConversationExists(gatheringId);
        }

        public Task<CoreConversation> GetConversationAsync(long conversationId)
        {
            return store.GetConversationAsync(conversationId);
        }

        public Task<List<CoreMembership>> GetConversationMembersAsync(long conversationId)
        {
            return store.GetConversationMembersAsync(conversationId);
        }

        public Task<int> GetLastPageNumber(long conversationId)
        {
            return store.GetLastPageNumber(conversationId);
        }

        public Task<List<CoreConversation>> GetConversationsForUserAsync(long userId)
        {
            return store.GetConversationsForUserAsync(userId);
        }

        public Task<CoreMembership> GetMembershipAsync(long conversationId, long userId)
        {
            return store.GetMembershipAsync(conversationId, userId);
        }

        public Task<List<MessageShard>> GetMessagesForConversationAsync(long conversationId, int pageNumber)
        {
            return store.GetMessagesForConversationAsync(conversationId, pageNumber);
        }

        public Task<CoreConversation> GetOrCreateGatheringConversation(long gatheringId)
        {
            return store.GetOrCreateGatheringConversation(gatheringId);
        }

        public Task<CoreConversation> GetOrCreateIndividualConversationBetween(long userIdA, long userIdB)
        {
            return store.GetOrCreateIndividualConversationBetween(userIdA, userIdB);
        }

        public Task<bool> IndividualConversationBetweenExists(long userIdA, long userIdB)
        {
            return store.IndividualConversationBetweenExists(userIdA, userIdB);
        }

        public Task RemoveUserFromConversationAsync(long conversationId, long userId)
        {
            return store.RemoveUserFromConversationAsync(conversationId, userId);
        }

        public Task UpdateConversationAsync(long conversationId, List<(string Property, object Value)> edits)
        {
            return store.UpdateConversationAsync(conversationId, edits);
        }

        public Task UpdateMembershipAsync(long conversationId, long userId, List<(string Property, object Value)> edits)
        {
            return store.UpdateMembershipAsync(conversationId, userId, edits);
        }

        public Task<MessageShard> GetCountMessagesSinceAsync(long conversationId)
        {
            return store.GetCountMessagesSinceAsync(conversationId);
        }
    }
}
