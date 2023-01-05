using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiscoverController : Controller
    {
        
        [HttpGet]
        public IActionResult IAmDiscover()
		{
			return new StatusCodeResult(418);
		}

        [HttpGet("{latitude}-{longitude}")]
        public IActionResult GetMapFiles(uint latitude, float longitude)
        {
            return Ok();
        }

    }

}
