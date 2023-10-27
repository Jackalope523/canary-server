using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Controllers
{
    [Route("error")]
    [ApiController]
    public class ErrorAgent : ControllerBase
    {

        [HttpGet]
        public IActionResult Error()
		{
			return new StatusCodeResult(418);
		}

    }

}
