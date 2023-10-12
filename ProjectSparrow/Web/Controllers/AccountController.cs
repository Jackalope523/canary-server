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

namespace Web.Controllers
{
    [Route("account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
		enum AccountError
		{
			MissingInformation,
            IncorrectCode,
            UserLockedOut,
			CouldNotLoginUser,
			CouldNotCreateUser,
            CouldNotModifyUser
		}

		IAccountOperations accounts;
        UserManager<UserShard> userManager;
        SignInManager<UserShard> signInManager;

        ISMSService smsService;
        IEmailService emailService;

        public AccountController(IAccountOperations accountOperations,
            UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
            ISMSService externalSMSService, IEmailService externalEmailService)
        {
            accounts = accountOperations;
            userManager = identityUserManager;
            signInManager = identitySignInManager;

            smsService = externalSMSService;
            emailService = externalEmailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount()
        {
            UserShard user;

            try
            {
                // Get current user
                // TODO Strip away internal data
                user = await GetCurrentUserAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Ok(user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AccountCredentialsModel credentials)
        {
            if (credentials == null || !ModelState.IsValid)
            {
                return BadRequest(AccountError.MissingInformation.ToString());
            }

            try
            {
                var user = await accounts.GetUserAsync(credentials.PhoneNumber);
                string code;

                // Verify that the account is activated
                if (await userManager.IsPhoneNumberConfirmedAsync(user))
                {
                    // Account is activated, generate regular 2FA token
                    code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                }
                else
				{
					// Account is not activated, generate change number token
					code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
				}
                
                // Send user an SMS with code
                await smsService.SendSMSAsync(user.PhoneNumber, $"Your Sparrow code is {code}");
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Ok();
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] AccountCredentialsModel credentials)
        {
			if (credentials == null || !ModelState.IsValid || credentials.Code == null)
            {
				return BadRequest(AccountError.MissingInformation.ToString());
			}

			try
			{
				var user = await accounts.GetUserAsync(credentials.PhoneNumber);
                
                if (await userManager.IsLockedOutAsync(user))
                {
                    return BadRequest(AccountError.UserLockedOut.ToString());
                }

                // Check if the account is activated
                if (await userManager.IsPhoneNumberConfirmedAsync(user))
                {
                    // Account is activated, check 2FA token validity
				    var result = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, credentials.Code);
                    if (result)
                    {
                        // Token matched, reset access tries and sign user in
                        await userManager.ResetAccessFailedCountAsync(user);
                        await signInManager.SignInAsync(user, false);
                    }
                    else
                    {
                        await userManager.AccessFailedAsync(user);
                        return BadRequest(AccountError.IncorrectCode.ToString());
                    }
                }
                else
				{
					// Account is not activated, check change number token validity
					var result = await userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, credentials.Code);
					if (result.Succeeded)
					{
                        // Token matched, reset access tries and sign user in
                        await userManager.ResetAccessFailedCountAsync(user);
						await signInManager.SignInAsync(user, false);

						if (!string.IsNullOrEmpty(user.Email))
						{
                            // Send verification email if an email is added
                            // TODO
							await emailService.SendEmailAsync(user.Email, "Welcome to Sparrow!", "Verify your Sparrow email.");
						}
					}
					else
					{
                        await userManager.AccessFailedAsync(user);
						return BadRequest(AccountError.IncorrectCode.ToString());
					}
				}
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] AccountSignUpModel details)
		{
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(AccountError.MissingInformation.ToString());
			}

            try
			{
                // Persist a new user
                await accounts.CreateUserAsync(details.PhoneNumber, details.Email ?? "",
                    details.Name, details.DateOfBirth.ToUniversalTime());
                
                // Send an SMS to new user with a generated change number token
                var user = await accounts.GetUserAsync(details.PhoneNumber);
				var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
				await smsService.SendSMSAsync(user.PhoneNumber, $"Your Sparrow code is {code}");
			}
            catch (InvalidUserException e)
			{
                // Account already exists
				var user = await accounts.GetUserAsync(details.PhoneNumber);

                // Check if account is activated
				if (!await userManager.IsPhoneNumberConfirmedAsync(user))
                {
                    // Account is not activated, send an SMS with a generated change number token
					var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
					await smsService.SendSMSAsync(user.PhoneNumber, $"Your Sparrow code is {code}");
				}

                return BadRequest(e.ToString());
            }
            catch (InvalidInformationException e)
            {
                return BadRequest(e.ToString());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ModifyAccount([FromBody] AccountDetailsModel details)
        {
			if (details == null || !ModelState.IsValid)
			{
				return BadRequest(AccountError.MissingInformation.ToString());
			}

            try
            {
                // Send updates to account manager
                var user = await GetCurrentUserAsync();
                await accounts.EditUserAsync(user.Id, name: details.Name);
            }
            catch (InvalidInformationException e)
            {
                return BadRequest(e.ToString());
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok();
        }

        private async Task<UserShard> GetCurrentUserAsync()
        {
            return await userManager.GetUserAsync(HttpContext.User);
        }
    }

}
