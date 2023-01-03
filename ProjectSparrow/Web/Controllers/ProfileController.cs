using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("accounts/[controller]")]
    [ApiController]
    public class ProfileController : Controller
	{
		private IAccountOperations accounts;

		public ProfileController(IAccountOperations accountOperations)
		{
			accounts = accountOperations;
		}

		[HttpGet]
        public IActionResult GetPage()
        {

            return Ok();
        }

        [HttpGet("following")]
        public IActionResult GetFollowed()
        {
            return Ok();
        }

		[HttpPost("following")]
		public IActionResult FollowUser()
		{
			return Ok();
		}

		[HttpPut("following")]
		public IActionResult UnfollowUser()
		{
			return Ok();
		}

		[HttpGet("blocked")]
		public IActionResult GetBlocked()
		{
			return Ok();
		}

		[HttpPost("blocked")]
		public IActionResult BlockUser()
		{
			return Ok();
		}

		[HttpPut("blocked")]
		public IActionResult UnblockUser()
		{
			return Ok();
		}
	}

}
