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
        string Title { get; set; }

        public long? GatheringId { get; init; }

        ////////
        // Synced Properties
        //////////////////////
        
        public Synced<List<(User User, CoreMembership Membership)>> Members { get; }
        public Synced<List<CoreMessage>> Messages { get; }

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

        public async Task MessageMembersAsync(User sender, CoreMessage message)
        {
            MessageShard msg = message.ToShard();

            var (onlineMembers, offlineMembers) = await (await Members).PartitionAsync(async (member) => await member.User.IsOnline());

            if (onlineMembers.Count > 0)
            {
                await Terminal.MessageDirector.SendClientMessageAsync(this, msg, onlineMembers.Select(u => u.User).ToArray());
            }

            if (offlineMembers.Count > 0)
            {
                var subscribedMembers = offlineMembers
                    .Where(member => !member.Membership.IsMuted)
                    .Select(u => u.User)
                    .ToArray();

                var shard = ToConversationShard();

                CanaryNotification notification = Type switch
                {
                    ConversationType.Individual => CanaryNotification.IndividualMessage(shard, sender.ToUserShard(), msg),
                    ConversationType.Group => CanaryNotification.GroupMessage(shard, sender.ToUserShard(), msg),
                    ConversationType.Gathering => CanaryNotification.GatheringMessage(shard, sender.ToUserShard(), message),
                };

                await User.NotifyAll(notification, subscribedMembers);
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
