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
    public class RootAgent : ControllerBase
    {

        [HttpGet]
        public IActionResult IAmRoot()
        {
            return new StatusCodeResult(418);
        }

    }
    
}
