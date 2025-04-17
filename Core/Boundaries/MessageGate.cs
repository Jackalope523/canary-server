using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

	public enum ChatType
	{
		Individual,
		Group,
		Gathering,
	}

	public enum MembershipType
	{
		Regular,
		Owner,
	}

	public enum MessageType
	{
		Activity,
		Text,
		Photo,
		ShareGathering,
		GatheringInvite,
		Snapshot,
		Nest,
	}

	public enum ActivityMessageType
	{ }

	public record CoreConversation(long Id, ChatType Type, string Title = default, long? GatheringId = null)
		: CoreOnlyData();
	public record ConversationShard(long Id, ChatType Type, string Title = default,
		long? GatheringId = null, bool? IsMuted = null, bool? HasUnread = null);

	public record CoreMembership(long UserId, MembershipType Type, DateTimeOffset LastSeen, bool IsMuted)
		: CoreOnlyData();
	public record MembershipShard(long UserId, MembershipType Type, DateTimeOffset LastSeen);

	public record MessageShard(long Id, long UserId, DateTimeOffset Timestamp, MessageType Type, object Value);

    #endregion

    #region Gates

    public interface IMessageDatabase
    {
		Task<CoreConversation> GetConversationAsync(long conversationId);

		Task<bool> IndividualConversationBetweenExists(long userIdA, long userIdB);
		Task<CoreConversation> GetOrCreateIndividualConversationBetween(long userIdA, long userIdB);

		Task<bool> GatheringConversationExists(long gatheringId);
		Task<CoreConversation> GetOrCreateGatheringConversation(long gatheringId);

		Task<long> CreateGroupChatConversationAsync(string title = default);

		Task<List<CoreConversation>> GetConversationsForUserAsync(long userId);
		Task<List<CoreMembership>> GetConversationMembersAsync(long conversationId);
		Task<CoreMembership> GetMembershipAsync(long conversationId, long userId);

		Task UpdateConversationAsync(long conversationId, List<(string Property, object Value)> edits);
		Task DeleteConversationAsync(long conversationId);

		Task AddUsersToConversationAsync(long conversationId, params long[] userIds);
		Task UpdateMembershipAsync(long conversationId, long userId, List<(string Property, object Value)> edits);
		Task RemoveUserFromConversationAsync(long conversationId, long userId);

		Task<List<MessageShard>> GetMessagesForConversationAsync(long conversationId, int startSeqId = 0, int endSeqId = 10);
        Task<MessageShard> AddMessageAsync(long conversationId, long userId, DateTimeOffset timestamp, MessageType type, object value);
    }

	public interface IMessageOperations
	{
		Task<List<ConversationShard>> GetConversationsAsync(long userId);
		Task<ConversationShard> GetConversationWithAsync(long userId, long targetId);

		Task<ConversationShard> GetConversationAsync(long userId, long conversationId);
		Task<List<MembershipShard>> GetMembersAsync(long userId, long conversationId);
		Task<List<MessageShard>> GetMessagesAsync(long userId, long conversationId);

		Task UserReadAsync(long userId, long conversationId);
		Task UserComposingAsync(long userId, long conversationId, bool isComposing);
		Task<MessageShard> SendTextAsync(long userId, long conversationId, string text);
		Task<MessageShard> SendPhotoAsync(long userId, long conversationId, MemoryStream photo);
		Task<MessageShard> InviteToGatheringAsync(long userId, long conversationId, long gatheringId);
		Task<MessageShard> ShareGatheringAsync(long userId, long conversationId, long gatheringId);
		Task<MessageShard> ShareSnapshotAsync(long userId, long conversationId, long snapshotId);
		Task<MessageShard> ShareNestAsync(long userId, long conversationId, long nestId);

		Task<ConversationShard> CreateGroupChatAsync(long userId, params long[] participantIds);
		Task EditGroupChatAsync(long userId, long conversationId,
			string title = "");
		Task DeleteGroupChatAsync(long userId, long conversationId);

		Task LeaveGroupChatAsync(long userId, long conversationId);
		Task SummonUserAsync(long userId, long conversationId, long targetId);
		Task KickUserAsync(long userId, long conversationId, long targetId);
	}

	public interface IMessageSocket
	{
		Task ReceiveMessage(long conversationId, MessageShard message);
		Task UserIsComposing(long userId, long conversationId, bool isComposing);
	}

	#endregion
}
