using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparrowServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorController : Controller
    {

        [HttpGet]
        public IActionResult GetPage()
        {

            return Ok();
        }

    }

}
