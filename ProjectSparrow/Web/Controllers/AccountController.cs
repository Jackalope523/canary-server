using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Web.Models;
using System.Net;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
		enum AccountError
		{
			MissingInformation,
			InvalidLoginCombination,
			CouldNotLoginUser,
			CouldNotCreateUser,
            CouldNotModifyUser
		}

		private IAccountOperations accounts;

        public AccountController(IAccountOperations accountOperations)
        {
            accounts = accountOperations;
        }

        [HttpGet]
        public IActionResult GetLoginToken([FromBody] AccountCredentialsModel credentials)
        {
            if (credentials == null || !ModelState.IsValid)
            {
                return BadRequest(AccountError.MissingInformation.ToString());
            }

            string token;

            try
            {
                token = accounts.TryLogin(credentials.Identification, credentials.Passkey);
            }
            catch
            {
                return BadRequest(AccountError.CouldNotLoginUser.ToString());
            }

            if (token == string.Empty)
            {
                return BadRequest(AccountError.InvalidLoginCombination.ToString());
            }
            else
            {
                return Ok(token);
            }
        }

        [HttpPut]
        public IActionResult ModifyAccount([FromBody] AccountDetailsModel details)
        {
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(AccountError.MissingInformation.ToString());
			}

            try
            {
                accounts.EditUser(details.Identification, details.Name, details.DateOfBirth, details.Photo);
            }
            catch
            {
                return BadRequest(AccountError.CouldNotModifyUser.ToString());
            }

            return Ok();
        }

        [HttpPost("sign-up")]
        public IActionResult CreateAccount([FromBody] AccountSignUpModel details)
		{
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(AccountError.MissingInformation.ToString());
			}

            try
			{
				accounts.CreateUser(details.Identification, details.Passkey, details.Name, details.DateOfBirth, details.Photo);
			}
            catch
            {
                return BadRequest(AccountError.CouldNotCreateUser.ToString());
            }

            return Ok();
        }
    }

}
