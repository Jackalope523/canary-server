using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using Microsoft.Extensions.Logging;
using Frontier.Manifests;
using Microsoft.AspNetCore.Authorization;

namespace Frontier.Controllers
{
    [Route("")]
	[AllowAnonymous]
    public class RootGuard : AbstractGuard
    {
		#region Initialisation

		public RootGuard(GuardBox box, UserManager<CoreUser> aspUserManager) : base(box, aspUserManager)
		{ }

		#endregion

		#region Actions

		[HttpGet]
        public IActionResult IAmRoot()
        {
            return new StatusCodeResult(418);
        }

		[HttpGet("req")]
		public IActionResult SparrowMinimumVersion()
		{
			return Ok(new SparrowDetailsManifest() { MinimumVersion = "0.0.1" });
		}

		#endregion
	}
}