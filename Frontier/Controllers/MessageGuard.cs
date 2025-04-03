using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http.Headers;
using Repository;
using Twilio.TwiML.Voice;
using System.Security.Cryptography;

namespace Frontier.Controllers
{
	[Route("messages")]
	public class MessageGuard : AbstractGuard
	{
		#region Initialisation

		public MessageGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

        #endregion

        #region Actions

        [HttpGet]
		public async Task<IActionResult> GetConversations()
        {
            return await Execute(async user =>
            {
                return await messages.GetConversationsAsync(user.Id);
            });
        }

        [HttpGet]
		public async Task<IActionResult> GetConversationWith(long target_id)
        {
            return await Execute(async user =>
            {
                return await messages.GetConversationWithAsync(user.Id, target_id);
            });
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversation(long conversationId)
        {
            return await Execute(async user =>
            {
                return await messages.GetConversationAsync(user.Id, conversationId);
            });
        }

        [HttpGet("{conversationId}/messages")]
		public async Task<IActionResult> GetConversationMessages(long conversationId)
        {
            return await Execute(async user =>
            {
                return await messages.GetMessagesAsync(user.Id, conversationId);
            });
        }

        [HttpGet("{conversationId}/members")]
		public async Task<IActionResult> GetConversationMembers(long conversationId)
        {
            return await Execute(async user =>
            {
                return await messages.GetMembersAsync(user.Id, conversationId);
            });
        }

        [HttpPost]
		public async Task<IActionResult> CreateGroupChat([FromForm] GroupChatManifest manifest)
        {
            if (manifest == null || !ModelState.IsValid)
            { return MissingInformation(); }

            return await Execute(async user =>
            {
                return await messages.CreateGroupChatAsync(user.Id, manifest.ParticipantIds);
            });
        }

        [HttpPost("{conversationId}")]
		public async Task<IActionResult> EditGroupChat(long conversationId, string title = null)
        {
            return await Execute(async user =>
            {
                await messages.EditGroupChatAsync(user.Id, conversationId,
                    title);
            });
        }

        [HttpPut("{conversationId}")]
		public async Task<IActionResult> LeaveGroupChat(long conversationId)
        {
            return await Execute(async user =>
            {
                await messages.LeaveGroupChatAsync(user.Id, conversationId);
            });
        }

        [HttpDelete("{conversationId}")]
		public async Task<IActionResult> DeleteGroupChat(long conversationId)
        {
            return await Execute(async user =>
            {
                await messages.DeleteGroupChatAsync(user.Id, conversationId);
            });
        }

        [HttpPost("{conversationId}/members")]
        public async Task<IActionResult> SummonUser(long conversationId, long target_id)
        {
            return await Execute(async user =>
            {
                await messages.SummonUserAsync(user.Id, conversationId, target_id);
            });
        }

        [HttpPut("{conversationId}/members")]
        public async Task<IActionResult> KickUser(long conversationId, long target_id)
        {
            return await Execute(async user =>
            {
                await messages.KickUserAsync(user.Id, conversationId, target_id);
            });
        }

        #endregion
    }
}