using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetLoginToken() // Takes in account model.
        {

            return Ok();
        }

        [HttpPut]
        public IActionResult ModifyAccount() // Takes in account and options models.
        {

            return Ok();
        }
    }

}
