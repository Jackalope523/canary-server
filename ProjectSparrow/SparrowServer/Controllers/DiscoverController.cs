using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparrowServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiscoverController : ControllerBase
    {
        
        [HttpGet]
        public IActionResult GetPage()
        {

            return Ok();
        }

        [HttpGet("{pos}")]
        public IActionResult GetMapFiles(uint pos) // Either will take in parameters or location model.
        {

            return Ok();
        }

    }

}
