using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("")]
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

		#endregion
	}
}