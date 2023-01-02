using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountOperations accounts;

        public AccountController(IAccountOperations accountOperations)
        {
            accounts = accountOperations;
        }

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
