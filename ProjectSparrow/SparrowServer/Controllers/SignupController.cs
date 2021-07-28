using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparrowServer.Controllers
{
    [Route("accounts/sign-up")]
    [ApiController]
    public class SignupController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetPage()
        {

            return Ok();
        }

        [HttpPost]
        public IActionResult CreateAccount() // Takes in account model.
        {

            return Ok();
        }

    }

}
