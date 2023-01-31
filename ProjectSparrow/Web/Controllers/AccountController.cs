using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Boundaries;
using Web.Models;
using System.Net;
using Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Web.Services;
using DataAccess.Entities;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
		enum AccountError
		{
			MissingInformation,
            IncorrectCode,
			CouldNotLoginUser,
			CouldNotCreateUser,
            CouldNotModifyUser
		}

		private IAccountOperations accounts;
        private UserManager<ThinUser> userManager;
        private SignInManager<ThinUser> signInManager;

        private ISMSService smsService;
        private IEmailService emailService;

        public AccountController(IAccountOperations accountOperations,
            UserManager<ThinUser> identityUserManager, SignInManager<ThinUser> identitySignInManager,
            ISMSService externalSMSService, IEmailService externalEmailService)
        {
            accounts = accountOperations;
            userManager = identityUserManager;
            signInManager = identitySignInManager;

            smsService = externalSMSService;
            emailService = externalEmailService;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] AccountCredentialsModel credentials)
        {
            if (credentials == null || !ModelState.IsValid)
            {
                return BadRequest(AccountError.MissingInformation.ToString());
            }

            try
            {
                var user = await accounts.GetUserAsync(credentials.PhoneNumber);

                var code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

                await smsService.SendSMSAsync(credentials.PhoneNumber, $"Your Sparrow code is {code}");
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("login/verify")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode([FromBody] AccountCredentialsModel credentials)
        {
			if (credentials == null || !ModelState.IsValid || credentials.Code == null)
            {
				return BadRequest(AccountError.MissingInformation.ToString());
			}

			try
			{
				var user = await accounts.GetUserAsync(credentials.PhoneNumber);
                
                if (await userManager.IsPhoneNumberConfirmedAsync(user))
                {
				    var result = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, credentials.Code);

                    if (result)
                    {
                        await signInManager.SignInAsync(user, false);
                    }
                    else
                    {
                        return BadRequest(AccountError.IncorrectCode.ToString());
                    }
                }
                else
				{
					var result = await userManager.ChangePhoneNumberAsync(user, credentials.PhoneNumber, credentials.Code);

					if (result.Succeeded)
					{
						await signInManager.SignInAsync(user, false);

						if (user.Email != null)
						{
							await emailService.SendEmailAsync(user.Email, "Welcome to Sparrow!", "Verify your Sparrow email.");
						}
					}
					else
					{
						return BadRequest(AccountError.IncorrectCode.ToString());
					}
				}
            }
            catch
            {
                return BadRequest(AccountError.CouldNotLoginUser.ToString());
            }

            return Ok();
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccountAsync([FromBody] AccountSignUpModel details)
		{
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(AccountError.MissingInformation.ToString());
			}

            try
			{
                await accounts.CreateUserAsync(details.PhoneNumber, details.Email ?? "",
                    details.Name, details.DateOfBirth);

                var user = await accounts.GetUserAsync(details.PhoneNumber);

                await userManager.UpdateSecurityStampAsync(user);

				var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, details.PhoneNumber);

				await smsService.SendSMSAsync(details.PhoneNumber, $"Your Sparrow code is {code}");
			}
            catch (InvalidUserException e)
			{
				var user = await accounts.GetUserAsync(details.PhoneNumber);

				if (!await userManager.IsPhoneNumberConfirmedAsync(user))
                {
					var code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

					await smsService.SendSMSAsync(details.PhoneNumber, $"Your Sparrow code is {code}");
				}

                return BadRequest(e.ToString());
            }
            catch (InvalidInformationException e)
            {
                return BadRequest(e.ToString());
            }
            catch
            {
                return BadRequest(AccountError.CouldNotCreateUser.ToString());
            }

            return Ok();
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyAccountAsync([FromBody] AccountDetailsModel details)
        {
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(AccountError.MissingInformation.ToString());
			}

            try
            {
                var user = await GetCurrentUserAsync();

                await accounts.EditUserAsync(user.Id, details.Name);
            }
            catch (InvalidInformationException e)
            {
                return BadRequest(e.ToString());
            }
            catch
            {
                return BadRequest(AccountError.CouldNotModifyUser.ToString());
            }

            return Ok();
        }

        private async Task<ThinUser> GetCurrentUserAsync()
        {
            return await userManager.GetUserAsync(HttpContext.User);
        }
    }

}
