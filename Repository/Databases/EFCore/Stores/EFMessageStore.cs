namespace Repository
{
    class EFMessageStore : QueryStore, IMessageDatabase
    {
        public EFMessageStore(Harbor.Flag flag) : base(flag)
        {
        }


        public async Task<MessageShard> AddMessageAsync(long conversationId, long userId, DateTimeOffset timestamp, MessageType type, object value)
        {
            Message toAdd;

            switch (type)
            {
                case MessageType.Text:
                    toAdd = new TextMessage()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Timestamp = timestamp,
                        Text = (string)value,
                    };
                    break;
                case MessageType.Photo:
                    toAdd = new ImageMessage()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Timestamp = timestamp,
                        ImageURL = (string)value,
                    };
                    break;
                case MessageType.ShareGathering:
                    toAdd = new GatheringShareMessage()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Timestamp = timestamp,
                        GatheringId = (long)value
                    };
                    break;
                case MessageType.GatheringInvite:
                    toAdd = new GatheringInviteMessage()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Timestamp = timestamp,
                        GatheringId = (long)value
                    };
                    break;
                case MessageType.Snapshot:
                    toAdd = new SnapshotMessage()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Timestamp = timestamp,
                        SnapshotId = (long)value
                    };
                    break;
                case MessageType.Nest:
                    toAdd = new ProfileMessage()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Timestamp = timestamp,
                        ProfileId = (long)value
                    };
                    break;
                default:
                    throw new InvalidInputException("Message of type \"" + type.ToString() + "\" is not supported in this method.");
            }

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Messages.Add(toAdd));
            return new MessageShard(toAdd.Id, toAdd.UserId, toAdd.Timestamp, type, value);
        }

        public Task AddUsersToConversationAsync(long conversationId, params long[] userIds)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreateConversationAsync(ConversationType type, string title = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteConversationAsync(long conversationId)
        {
            throw new NotImplementedException();
        }

        public Task<CoreConversation> GetConversationAsync(long conversationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CoreMembership>> GetConversationMembersAsync(long conversationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CoreConversation>> GetConversationsForUserAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<CoreMembership> GetMembershipAsync(long conversationId, long userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<MessageShard>> GetMessagesForConversationAsync(long conversationId, int startSeqId = 0, int endSeqId = 10)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromConversationAsync(long conversationId, long userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateConversationAsync(long conversationId, List<(string Property, object Value)> edits)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMembershipAsync(long conversationId, long userId, List<(string Property, object Value)> edits)
        {
            throw new NotImplementedException();
        }
    }
}
