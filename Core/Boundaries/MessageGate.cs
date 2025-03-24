using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Core.Boundaries
{
    #region Schemas

	public enum ConversationType
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

	public record CoreConversation(long Id, ConversationType Type, string Title = default, long? GatheringId = null)
		: CoreOnlyData();
	public record ConversationShard(long Id, ConversationType Type, string Title = default,
		long? GatheringId = null, bool? IsMuted = null, bool? HasUnread = null);

	public record CoreMembership(long UserId, MembershipType Type, DateTimeOffset LastSeen, bool IsMuted)
		: CoreOnlyData();
	public record MembershipShard(long UserId, MembershipType Type, DateTimeOffset LastSeen);

	public record CoreMessage(int SequenceId, long UserId, DateTimeOffset Timestamp, MessageType Type, object Value)
		: CoreOnlyData();
	public record MessageShard(int SequenceId, long UserId, DateTimeOffset Timestamp, MessageType Type, object Value);

    #endregion

    #region Gates

    public interface IMessageDatabase
    {
		Task<CoreConversation> GetConversationAsync(long conversationId);
		Task<List<CoreConversation>> GetConversationsForUserAsync(long userId);
		Task<List<CoreMembership>> GetConversationMembersAsync(long conversationId);
		Task<CoreMembership> GetMembershipAsync(long conversationId, long userId);

		Task<long> CreateConversationAsync(ConversationType type, string title = default);
		Task UpdateConversationAsync(long conversationId, List<(string Property, object Value)> edits);
		Task DeleteConversationAsync(long conversationId);

		Task AddUsersToConversationAsync(long conversationId, params long[] userIds);
		Task UpdateMembershipAsync(long conversationId, long userId, List<(string Property, object Value)> edits);
		Task RemoveUserFromConversationAsync(long conversationId, long userId);

		Task<List<CoreMessage>> GetMessagesForConversationAsync(long conversationId, int startSeqId = 0, int endSeqId = 10);
        Task AddMessageAsync(long conversationId, long userId, long sequenceId, DateTimeOffset timestamp, MessageType type, string value);
    }

	public interface IMessageOperations
	{
		Task<List<ConversationShard>> GetConversationsAsync(long userId);
		Task<List<MembershipShard>> GetConversationMembersAsync(long userId, long conversationId);
		Task<List<MessageShard>> GetMessagesAsync(long userId, long conversationId);

		Task UserReadAsync(long userId, long conversationId);
		Task UserComposingAsync(long userId, long conversationId);
		Task SendTextAsync(long userId, long conversationId, string text);
		Task SendPhotoAsync(long userId, long conversationId, MemoryStream photo);
		Task ShareGatheringAsync(long userId, long conversationId, long gatheringId);
		Task ShareSnapshotAsync(long userId, long conversationId, long snapshotId);
		Task ShareNestAsync(long userId, long conversationId, long nestId);

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
		Task UserIsComposing(long userId, bool isComposing);
	}

	#endregion
}
