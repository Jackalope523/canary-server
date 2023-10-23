using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorController : Controller
    {

        [HttpGet]
        public IActionResult Error()
		{
			return new StatusCodeResult(418);
		}

    }

}
