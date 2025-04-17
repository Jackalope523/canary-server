using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;
using System.Linq;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;
using static Core.Entities.Smithing;

namespace Core.Controls
{
    internal class MessageDirector : AbstractDirector, IMessageOperations
    {
        #region Initialisation

        public MessageDirector(CoreTerminal terminal) : base(terminal) { }

        #endregion

        #region Operations

        public async Task<List<ConversationShard>> GetConversationsAsync(long userId)
        {
            var user = await GetUserAsync(userId);

            return (await user.Conversations)
                .ConvertAll(c => c.Conversation.ToConversationShard(c.Membership));
        }

        public async Task<ConversationShard> GetConversationWithAsync(long userId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var target = await GetUserAsync(targetId);

            // todo checks

            Conversation conversation = new(await Messages.GetOrCreateIndividualConversationBetween(userId, targetId));

            return conversation.ToConversationShard();
        }

        public async Task<ConversationShard> GetConversationAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            return conversation.ToConversationShard();
        }

        public async Task<List<MembershipShard>> GetMembersAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            return (await conversation.Members)
                .ConvertAll(m => m.Membership.ToShard());
        }

        public async Task<List<MessageShard>> GetMessagesAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            return await conversation.Messages;
        }

        public async Task UserReadAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            await Messages.UpdateMembershipAsync(conversation.Id, user.Id, new() { (nameof(CoreMembership.LastSeen), Time) });
        }

        public async Task UserComposingAsync(long userId, long conversationId, bool isComposing)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            _ = conversation.IndicateUserComposingAsync(user, isComposing);
        }

        public async Task<MessageShard> SendTextAsync(long userId, long conversationId, string text)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Text, text);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);

            return message;
        }

        public async Task<MessageShard> SendPhotoAsync(long userId, long conversationId, MemoryStream photo)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Photo, photo);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);

            return message;
        }

        public async Task<MessageShard> InviteToGatheringAsync(long userId, long conversationId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            var gathering = await GetGatheringAsync(gatheringId);

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.GatheringInvite, gathering.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);

            return message;
        }

        public async Task<MessageShard> ShareGatheringAsync(long userId, long conversationId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            var gathering = await GetGatheringAsync(gatheringId);

            Verify(await user.CanView(gathering),
                new UserErrorException(GatheringErrorCode.CANNOT_VIEW));

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.ShareGathering, gathering.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);

            return message;
        }

        public async Task<MessageShard> ShareSnapshotAsync(long userId, long conversationId, long snapshotId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);
            User snapshotOwner = await GetUserAsync(snapshot.User.Id);
            var etchedGathering = await GetGatheringAsync(snapshot.GatheringId);

            Verify(user.Taken(snapshot) ||
                await user.IsCompanionsWith(snapshotOwner) ||
                await etchedGathering.HasOnGuestList(user),
                new UserErrorException(SnapshotErrorCode.CANNOT_VIEW));

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Snapshot, snapshot.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);

            return message;
        }

        public async Task<MessageShard> ShareNestAsync(long userId, long conversationId, long nestId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            var nest = await GetUserAsync(nestId);

            FailIf(await user.IsBlockedBy(nest),
                new UserErrorException(UserErrorCode.CANNOT_VIEW));

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Nest, nest.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);

            return message;
        }

        public async Task<ConversationShard> CreateGroupChatAsync(long userId, params long[] participantIds)
        {
            var user = await GetUserAsync(userId);

            var conversationId = await Messages.CreateGroupChatConversationAsync();

            // todo only add applicable users

            var newConversation = await GetConversationAsync(conversationId);

            return newConversation.ToConversationShard();
        }

        public async Task EditGroupChatAsync(long userId, long conversationId, string title = "")
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            Verify(await conversation.IsModifiableBy(user),
                new UserErrorException(ConversationErrorCode.CANNOT_EDIT_PERMISSION));

            Conversation editedConversation = new(conversation.ToCoreConversation())
            {
                Title = title,
            };

            // Validate conversation
            Verify(editedConversation.ValidateAndNormalise(out string issues),
                new UserErrorException(ConversationErrorCode.INVALID_DETAILS, new { issues }));

            List<(string Property, object Value)> edits = new();
            List<object> editMessages = new();

            if (!string.IsNullOrEmpty(title))
            {
                edits.Add((nameof(CoreConversation.Title), editedConversation.Title));
                editMessages.Add(editedConversation.Title); // todo activity messages
            }

            if (edits.Count > 0)
            {
                await Messages.UpdateConversationAsync(conversation.Id, edits);

                foreach (var value in editMessages)
                {
                    var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, value);
                    _ = conversation.MessageOthersAsync(User.Hollow, message);
                }
            }
        }

        public async Task DeleteGroupChatAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(conversation.Type == ChatType.Group,
                new UserErrorException(ConversationErrorCode.NOT_GROUP_CHAT));

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            Verify(await conversation.IsModifiableBy(user),
                new UserErrorException(ConversationErrorCode.CANNOT_DELETE_PERMISSION));

            await Messages.DeleteConversationAsync(conversation.Id);
        }

        public async Task LeaveGroupChatAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(conversation.Type == ChatType.Group,
                new UserErrorException(ConversationErrorCode.NOT_GROUP_CHAT));

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            await Messages.RemoveUserFromConversationAsync(conversation.Id, user.Id);

            var activityMessage = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, null);
            _ = conversation.MessageOthersAsync(User.Hollow, activityMessage);
        }

        public async Task SummonUserAsync(long userId, long conversationId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var summoned = await GetUserAsync(targetId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(conversation.Type == ChatType.Group,
                new UserErrorException(ConversationErrorCode.NOT_GROUP_CHAT));

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            Verify(await user.IsCompanionsWith(summoned),
                new UserErrorException(ConversationErrorCode.CANNOT_INVITE_NEUTRAL));

            await Messages.AddUsersToConversationAsync(conversation.Id, summoned.Id);

            var activityMessage = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, null);
            _ = conversation.MessageOthersAsync(User.Hollow, activityMessage);
        }

        public async Task KickUserAsync(long userId, long conversationId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(conversation.Type == ChatType.Group,
                new UserErrorException(ConversationErrorCode.NOT_GROUP_CHAT));

            Verify(await conversation.HasMember(user),
                new UserErrorException(ConversationErrorCode.NOT_MEMBER));

            Verify(await conversation.IsModifiableBy(user),
                new UserErrorException(ConversationErrorCode.CANNOT_KICK_PERMISSION));

            await Messages.RemoveUserFromConversationAsync(conversation.Id, targetId);

            var activityMessage = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, null);
            _ = conversation.MessageOthersAsync(User.Hollow, activityMessage);
        }

        #endregion

        #region Favours

        public async Task<List<(Conversation, CoreMembership)>> RequestConversationsForUserAsync(User user)
        {
            var conversations = await Messages.GetConversationsForUserAsync(user.Id);
            var pairs = await Psijic.Once(conversations
                .Select(async c => (new Conversation(c), await Messages.GetMembershipAsync(c.Id, user.Id))));

            return pairs.ToList();
        }

        public async Task<List<(User, CoreMembership)>> RequestConversationMembersAsync(Conversation conversation)
        {
            var members = await Messages.GetConversationMembersAsync(conversation.Id);
            var pairs = await Psijic.Once(members
                .Select(async m => (await User.GetUserAsync(m.UserId), m)));

            return pairs.ToList();
        }

        public async Task<List<MessageShard>> RequestConversationMessagesAsync(Conversation conversation)
        {
            return await Messages.GetMessagesForConversationAsync(conversation.Id);
        }

        public async Task SendClientMessageAsync(Conversation conversation, MessageShard message, params User[] users)
        {
            string[] connectionIds = (await Psijic.Once(users
                .Select(async u => await u.Connections)))
                .SelectMany(c => c)
                .ToArray();

            await Terminal.SocketService.BroadcastAsync(client => client.ReceiveMessage(conversation.Id, message),
                connectionIds);
        }

        #endregion

        #region Tools

        private async Task<Conversation> GetConversationAsync(long conversationId)
        {
            return new(await Messages.GetConversationAsync(conversationId));
        }

        #endregion
    }
}
