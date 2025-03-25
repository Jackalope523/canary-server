using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Boundaries;

using static Core.Entities.Arbiter;
using static Core.Entities.Psijic;
using Microsoft.Extensions.Logging;
using Core.Notifications;
using Microsoft.VisualBasic;
using System.Reflection;

namespace Core.Entities
{
    using static CoreTerminal;

    internal class Conversation
    {
		#region Variables

		//////
		// Constants
		//////////////

		public const int MaximumTitleLength = 30;

		///////
		// Properties
		///////////////

		public long Id { get; init; }
        public ConversationType Type { get; init; }
        public string Title { get; set; }

        public long? GatheringId { get; init; }

        ////////
        // Synced Properties
        //////////////////////
        
        public Synced<List<(User User, CoreMembership Membership)>> Members { get; }
        public Synced<List<MessageShard>> Messages { get; }

        public Synced<Gathering> Gathering { get; }

        #endregion

        #region Initialisation & Extraction

        public Conversation()
        {
            Members = new(() => Terminal.MessageDirector.RequestConversationMembersAsync(this));
            Messages = new(() => Terminal.MessageDirector.RequestConversationMessagesAsync(this));

            Gathering = new(() => GatheringId.HasValue ? Entities.Gathering.GetGatheringAsync(GatheringId.Value) : Task.FromResult(Entities.Gathering.None));
        }

        public Conversation(CoreConversation fromConversation) : this()
        {
            Id = fromConversation.Id;
            Type = fromConversation.Type;
            Title = fromConversation.Title;
        }

        public CoreConversation ToCoreConversation()
        {
            return new(Id, Type, Title);
        }

        public ConversationShard ToConversationShard()
        {
            return new(Id, Type, Title, GatheringId);
        }

        public async Task<ConversationShard> ToConversationShard(User relativeTo)
        {
            Verify(await HasMember(relativeTo),
                new UnexpectedFailureException("ToConversationShard: User is not member"));

            var userMembership = (await Members).Find(member => member.User.Equals(relativeTo));

            return new(Id, Type, Title, GatheringId,
                IsMuted: userMembership.Membership.IsMuted); // todo fill already read indicator
        }

        public ConversationShard ToConversationShard(CoreMembership relativeTo)
        {
            return new(Id, Type, Title, GatheringId,
                IsMuted: relativeTo.IsMuted); // todo fill already read indicator
        }

		#endregion

		#region Composition

		public bool ValidateAndNormalise(out string issues)
        {
            issues = "";

            // Sanitise
            Title = ContentValidation.NormaliseText(Title, MaximumTitleLength);
            if (string.IsNullOrEmpty(Title)) { issues += "Title cannot be empty. "; }

            return issues.Equals("");
        }

		#endregion

		#region Checks

        public async Task<bool> IsModifiableBy(User user)
        {
            Verify(await HasMember(user),
                new UnexpectedFailureException("IsModifiableBy: User is not member"));

            // Check if user has priviledges
            var userMembership = (await Members).Find(member => member.User.Equals(user));

            if (Type == ConversationType.Individual ||
                userMembership.Membership.Type.Equals(MembershipType.Owner))
			{ return true; }

			return false;
        }

        public async Task<bool> HasMember(User user)
        {
            // Check if user is affiliated with conversation
            return (await Members).Exists(u => u.User.Equals(user));
        }

        #endregion

        #region Effects

        #endregion

        #region Actions

        public async Task MessageOthersAsync(User sender, MessageShard message)
        {
            var otherMembers = (await Members).Where(m => !m.User.Equals(sender));

            var (onlineMembers, _) = await otherMembers.PartitionAsync(async (member) => await member.User.IsOnline());

            if (onlineMembers.Any())
            {
                await Terminal.MessageDirector.SendClientMessageAsync(this, message, onlineMembers.Select(u => u.User).ToArray());
            }
        }

        public async Task MessageOrNotifyOthersAsync(User sender, MessageShard message)
        {
            var otherMembers = (await Members).Where(m => !m.User.Equals(sender));

            var (onlineMembers, offlineMembers) = await otherMembers.PartitionAsync(async (member) => await member.User.IsOnline());

            if (onlineMembers.Any())
            {
                await Terminal.MessageDirector.SendClientMessageAsync(this, message, onlineMembers.Select(u => u.User).ToArray());
            }

            if (offlineMembers.Any())
            {
                var subscribedMembers = offlineMembers
                    .Where(member => !member.Membership.IsMuted)
                    .Select(u => u.User)
                    .ToArray();

                var shard = ToConversationShard();

                CanaryNotification notification = Type switch
                {
                    ConversationType.Individual => CanaryNotification.IndividualMessage(shard, sender.ToUserShard(), message),
                    ConversationType.Group => CanaryNotification.GroupMessage(shard, sender.ToUserShard(), message),
                    ConversationType.Gathering => CanaryNotification.GatheringMessage(await (await Gathering).ToGatheringShard(), shard, sender.ToUserShard(), message),
                    _ => throw new UnexpectedFailureException("ConversationType does not exist"),
                };

                await User.NotifyAll(notification, subscribedMembers);
            }
        }

        public async Task IndicateUserComposingAsync(User user, bool isComposing)
        {
            var otherMembers = (await Members).Where(m => !m.User.Equals(user));

            var (onlineMembers, _) = await otherMembers.PartitionAsync(async (member) => await member.User.IsOnline());

            if (onlineMembers.Any())
            {
                var connections = (await Psijic.Once(onlineMembers
                    .Select(async u => await u.User.Connections)))
                    .SelectMany(c => c)
                    .ToArray();

                await Terminal.SocketService.BroadcastAsync(client => client.UserIsComposing(user.Id, Id, isComposing), connections);
            }
        }

        #endregion

        #region Dissimilation

        public override bool Equals(object obj)
		{
			return obj is Conversation other &&
                Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion
	}
}
