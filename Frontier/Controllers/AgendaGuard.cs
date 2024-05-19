using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;

using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("agenda")]
    public class AgendaGuard : AbstractGuard
	{
		#region Initialisation

		public AgendaGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

        #endregion

        #region Actions

        [HttpGet("{targetIdentification}")]
        public async Task<IActionResult> GetUserAgenda(ulong targetIdentification)
        {
            return await Execute(async user =>
                await profiles.GetUserAgendaAsync(user.Id, targetIdentification));
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendAgenda()
        {
            return await Execute(async user =>
                await profiles.GetFriendAgendaAsync(user.Id));
        }

        #endregion
    }
}