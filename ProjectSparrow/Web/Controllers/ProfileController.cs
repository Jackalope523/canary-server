using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GetPage()
        {

            return Ok();

            // return View(); // Can mess with this at a later date.
        }

    }

}
