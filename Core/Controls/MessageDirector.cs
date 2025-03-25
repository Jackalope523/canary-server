using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Boundaries;
using System.Collections.Concurrent;
using Core.Entities;
using System.Linq;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;
using static Core.Entities.Smithing;
using static System.Net.Mime.MediaTypeNames;

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

        public async Task<List<MembershipShard>> GetConversationMembersAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            return (await conversation.Members)
                .ConvertAll(m => m.Membership.ToShard());
        }

        public async Task<List<MessageShard>> GetMessagesAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            return (await conversation.Messages)
                .ConvertAll(m => m.ToShard());
        }

        public async Task UserReadAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            await Messages.UpdateMembershipAsync(conversation.Id, user.Id, new() { (nameof(CoreMembership.LastSeen), Time) });
        }

        public async Task UserComposingAsync(long userId, long conversationId, bool isComposing)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            _ = conversation.IndicateUserComposingAsync(user, isComposing);
        }

        public async Task SendTextAsync(long userId, long conversationId, string text)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Text, text);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);
        }

        public async Task SendPhotoAsync(long userId, long conversationId, MemoryStream photo)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Photo, photo);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);
        }

        public async Task InviteToGatheringAsync(long userId, long conversationId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            var gathering = await GetGatheringAsync(gatheringId);

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.GatheringInvite, gathering.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);
        }

        public async Task ShareGatheringAsync(long userId, long conversationId, long gatheringId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            var gathering = await GetGatheringAsync(gatheringId);

            Verify(await user.CanView(gathering),
                new UserErrorException());

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.ShareGathering, gathering.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);
        }

        public async Task ShareSnapshotAsync(long userId, long conversationId, long snapshotId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            var snapshot = await Snapshots.GetSnapshotAsync(snapshotId);

            Verify(user.Taken(snapshot),
                new UserErrorException());

            // todo verifications

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Snapshot, snapshot.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);
        }

        public async Task ShareNestAsync(long userId, long conversationId, long nestId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            var nest = await GetUserAsync(nestId);

            // todo verifications

            var message = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Nest, nest.Id);

            _ = conversation.MessageOrNotifyOthersAsync(user, message);
        }

        public async Task<ConversationShard> CreateGroupChatAsync(long userId, params long[] participantIds)
        {
            var user = await GetUserAsync(userId);

            var conversationId = await Messages.CreateConversationAsync(ConversationType.Group);

            // todo add applicable users

            var newConversation = await GetConversationAsync(conversationId);

            return newConversation.ToConversationShard();
        }

        public async Task EditGroupChatAsync(long userId, long conversationId, string title = "")
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            Verify(await conversation.IsModifiableBy(user),
                new UserErrorException());

            Conversation editedConversation = new(conversation.ToCoreConversation())
            {
                Title = title,
            };

            // Validate conversation
            Verify(editedConversation.ValidateAndNormalise(out string issues),
                new UserErrorException(, new { issues }));

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

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            Verify(conversation.Type == ConversationType.Group,
                new UserErrorException());

            Verify(await conversation.IsModifiableBy(user),
                new UserErrorException());

            await Messages.DeleteConversationAsync(conversation.Id);
        }

        public async Task LeaveGroupChatAsync(long userId, long conversationId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            Verify(conversation.Type == ConversationType.Group,
                new UserErrorException());

            await Messages.RemoveUserFromConversationAsync(conversation.Id, user.Id);

            var activityMessage = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, );
            _ = conversation.MessageOthersAsync(User.Hollow.Id, activityMessage);
        }

        public async Task SummonUserAsync(long userId, long conversationId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            FailIf(conversation.GatheringId.HasValue,
                new UserErrorException());

            // todo other verifications

            await Messages.AddUsersToConversationAsync(conversation.Id, targetId);

            var activityMessage = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, );
            _ = conversation.MessageOthersAsync(User.Hollow.Id, activityMessage);
        }

        public async Task KickUserAsync(long userId, long conversationId, long targetId)
        {
            var user = await GetUserAsync(userId);
            var conversation = await GetConversationAsync(conversationId);

            Verify(await conversation.HasMember(user),
                new UserErrorException());

            Verify(conversation.Type == ConversationType.Group,
                new UserErrorException());

            Verify(await conversation.IsModifiableBy(user),
                new UserErrorException());

            await Messages.RemoveUserFromConversationAsync(conversation.Id, targetId);

            var activityMessage = await Messages.AddMessageAsync(conversation.Id, user.Id, Time, MessageType.Activity, );
            _ = conversation.MessageOthersAsync(User.Hollow.Id, activityMessage);
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

        public async Task<List<CoreMessage>> RequestConversationMessagesAsync(Conversation conversation)
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

        public async Task<Conversation> GetConversationAsync(long conversationId)
        {
            return new(await Messages.GetConversationAsync(conversationId));
        }

        #endregion
    }
}
