using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Controllers
{
    [Route("")]
    [ApiController]
    public class RootController : Controller
    {

        [HttpGet]
        public IActionResult IAmRoot()
        {
            return new StatusCodeResult(418);
        }

    }
    
}
