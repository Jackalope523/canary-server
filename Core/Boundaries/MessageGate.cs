using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		Admin,
		Regular,
	}

	public enum MessageType
	{
		Activity,
		Message,
		Photo,
		ShareGathering,
		GatheringInvite,
		Snapshot,
		Nest,
	}

	public record CoreConversation(long Id, ConversationType Type, string Title = default)
		: CoreOnlyData();
	public record ConversationShard(long Id, ConversationType Type, bool IsMuted, bool HasUnread, string Title = default);

	public record CoreMembership(long UserId, MembershipType Type, DateTimeOffset LastSeen, bool IsMuted, bool IsConnected)
		: CoreOnlyData();
	public record MembershipShard(long UserId, MembershipType Type, DateTimeOffset LastSeen);

	public record CoreMessage(int SequenceId, long UserId, DateTimeOffset Timestamp, MessageType Type, object Value)
		: CoreOnlyData();
	public record MessageShard(int SequenceId, long UserId, DateTimeOffset Timestamp, MessageType Type, object Value);

    #endregion

    #region Gates

    public interface IMessageDatabase
    {
		Task<List<string>> GetConnectionsAsync(long userId);
		Task<Dictionary<long, List<string>>> GetConnectionsAsync(params long[] userIds);
		Task AddConnectionAsync(long userId, string connectionId);
		Task DeleteConnectionAsync(string connectionId);

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

		Task UserConnectedAsync(long userId, string connectionId);
		Task UserDisconnectedAsync(long userId, string connectionId);

		Task ProcessMessageAsync(long userId);
		Task UserComposingAsync(long userId);

		Task CreateGroupChatAsync(long userId);
		Task DeleteGroupChatAsync(long userId);
		Task InviteToGroupChatAsync(long userId);
		Task LeaveGroupChatAsync(long userId);
	}

	public interface IMessageSocket
	{

	}

	#endregion
}
