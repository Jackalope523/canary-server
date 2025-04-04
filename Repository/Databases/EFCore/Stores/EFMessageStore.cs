using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static Repository.ConversationLink;

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
            return new MessageShard(toAdd.Id, toAdd.UserId ?? 0, toAdd.Timestamp, type, value);
        }

        public async Task AddUsersToConversationAsync(long conversationId, params long[] userIds)
        {
            Discussion discussion = storeSentry.BeginDiscussion();

            foreach (long userId in userIds)
            {
                storeSentry.DiscussWrite(ctx => 
                    ctx.ConversationLinks.
                    Add(
                        new ConversationLink 
                        { 
                            ConversationId = conversationId, 
                            UserId = userId,
                            LastSeen = DateTime.UtcNow,
                            Type = MembershipType.Regular
                        }
                ), discussion);
            }

            await storeSentry.EndDiscussionAsync(discussion);
        }

        public async Task<long> CreateConversationAsync(ConversationType type, string title)
        {
            Conversation toAdd = new() { Title = title, Type = type };

            await storeSentry.ExecuteWriteAsync(ctx => ctx.Conversations.Add(toAdd));

            return toAdd.Id;
        }

        public async Task DeleteConversationAsync(long conversationId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Messages.
               Where(m => m.ConversationId == conversationId).
               ExecuteUpdateAsync(setter => setter.SetProperty(s => s.SoftDeleted, true)));

            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.ConversationLinks.
               Where(l => l.ConversationId == conversationId).
               ExecuteUpdateAsync(setter => setter.SetProperty(s => s.SoftDeleted, true)));

            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.Conversations.
               Where(c => c.Id == conversationId).
               ExecuteUpdateAsync(setter => setter.SetProperty(s => s.SoftDeleted, true)));
        }

        public async Task<CoreConversation> GetConversationAsync(long conversationId)
        {
            Conversation conversation = await storeSentry.ExecuteReadAsync(ctx =>
                                            ctx.Conversations.
                                            Where(c => c.Id == conversationId).
                                            SingleAsync());

            return new CoreConversation(conversation.Id, conversation.Type, conversation.Title, conversation.GatheringId);
        }

        public async Task<List<CoreMembership>> GetConversationMembersAsync(long conversationId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
                    ctx.ConversationLinks.
                    Where(l => l.ConversationId == conversationId).
                    Select(l => new CoreMembership(l.UserId, l.Type, l.LastSeen, l.Muted)).
                    ToListAsync());
        }

        public async Task<List<CoreConversation>> GetConversationsForUserAsync(long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx =>
                    ctx.ConversationLinks.
                    Where(l => l.UserId == userId).
                    Join(
                        ctx.Conversations,
                        l => l.ConversationId,
                        c => c.Id,
                        (l, c) => new CoreConversation(c.Id, c.Type, c.Title, c.GatheringId)
                        ).
                    ToListAsync());
        }

        public async Task<CoreMembership> GetMembershipAsync(long conversationId, long userId)
        {
            return await storeSentry.ExecuteReadAsync(ctx => 
                    ctx.ConversationLinks.
                    Where(l => l.ConversationId == conversationId && l.UserId == userId).
                    Select(l => new CoreMembership(l.UserId, l.Type, l.LastSeen, l.Muted)).
                    SingleAsync());
        }

        public async Task<List<MessageShard>> GetMessagesForConversationAsync(long conversationId, int startSeqId = 0, int endSeqId = 10)
        {
            List<Message> messages =  await storeSentry.ExecuteReadAsync(ctx =>
                                        ctx.Messages.
                                        Where(m => m.ConversationId == conversationId).
                                        ToListAsync());

            List<MessageShard> toReturn = new();
            foreach (Message message in messages)
            {
                MessageShard messageShard = new(message.Id, message.UserId ?? 0, message.Timestamp, message.Type, null);
                switch (message) 
                {
                    case TextMessage textMessage:
                        toReturn.Add(messageShard with { Value = textMessage.Text });
                        break;
                    case ImageMessage imageMessage:
                        toReturn.Add(messageShard with { Value = imageMessage.ImageURL });
                        break;
                    case GatheringShareMessage gatheringShareMessage:
                        toReturn.Add(messageShard with { Value = gatheringShareMessage.GatheringId });
                        break;
                    case GatheringInviteMessage gatheringInviteMessage:
                        toReturn.Add(messageShard with { Value = gatheringInviteMessage.GatheringId });
                        break;
                    case ProfileMessage profileMessage:
                        toReturn.Add(messageShard with { Value = profileMessage.ProfileId });
                        break;
                    case SnapshotMessage snapshotMessage:
                        toReturn.Add(messageShard with { Value = snapshotMessage.SnapshotId });
                        break;
                    default:
                        throw new ArgumentException("Message of type " + message.GetType().Name + " is not supported by this method.");
                }
            }

            return toReturn;
        }

        public async Task RemoveUserFromConversationAsync(long conversationId, long userId)
        {
            await storeSentry.ExecuteWriteAsync(ctx =>
               ctx.ConversationLinks.
               Where(l => l.ConversationId == conversationId && l.UserId == userId).
               ExecuteUpdateAsync(setter => setter.SetProperty(s => s.SoftDeleted, true)));
        }

        public async Task UpdateConversationAsync(long conversationId, List<(string Property, object Value)> edits)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            Conversation c = new() { Id = conversationId };
            storeSentry.DiscussWrite(ctx => ctx.Conversations.Attach(c), currentDiscussion);

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case nameof(CoreConversation.Title):
                        c.Title = (string)Value;
                        break;
                    case nameof(CoreConversation.Type):
                        c.Type = (ConversationType)Value;
                        break;
                    case nameof(CoreConversation.GatheringId):
                        c.GatheringId = (long)Value;
                        break;
                    default:
                        throw new InvalidInputException($"Property named \"{Property}\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(c).Property(Property).IsModified = true, currentDiscussion);
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }

        public async Task UpdateMembershipAsync(long conversationId, long userId, List<(string Property, object Value)> edits)
        {
            Discussion currentDiscussion = storeSentry.BeginDiscussion();

            ConversationLink l = await storeSentry.ExecuteReadAsync(ctx => 
                                    ctx.ConversationLinks.
                                    Where(l => l.ConversationId == conversationId && l.UserId == userId).
                                    SingleAsync());

            storeSentry.DiscussWrite(ctx => ctx.ConversationLinks.Attach(l), currentDiscussion);

            foreach ((string Property, object Value) in edits)
            {
                switch (Property)
                {
                    case nameof(CoreMembership.IsMuted):
                        l.Muted = (bool)Value;
                        break;
                    case nameof(CoreMembership.LastSeen):
                        l.LastSeen = (DateTimeOffset)Value;
                        break;
                    case nameof(CoreMembership.Type):
                        l.Type = (MembershipType)Value;
                        break;
                    default:
                        throw new InvalidInputException($"Property named \"{Property}\" can not be updated using this method.");
                }
                storeSentry.DiscussWrite(ctx => ctx.Entry(l).Property(Property).IsModified = true, currentDiscussion);
            }
            await storeSentry.EndDiscussionAsync(currentDiscussion);
        }

        public async Task<CoreConversation> GetOrCreateIndividualConversationBetween(long userIdA, long userIdB)
        {
            throw new NotImplementedException();
        }
    }
}
