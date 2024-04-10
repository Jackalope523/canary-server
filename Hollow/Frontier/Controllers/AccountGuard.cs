using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Shared;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
    [Route("account")]
    public class AccountGuard : AbstractGuard
	{
		#region Initialisation

		public AccountGuard(ILogger logger,
            UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, IMediaOperations mediaOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(logger,
                identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				disciplineOperations, mediaOperations,
				notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpGet]
        public async Task<IActionResult> GetAccount()
        {
            return await Execute(async () =>
            {
                // Get current user
                SelfUserManifest user = new(await GetCurrentUserAsync());

                return user;
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AccountCredentialsManifest credentials)
        {
            // Verify parameters
            if (credentials == null || !ModelState.IsValid)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

            return await Execute(async () =>
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
            });
        }

        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            return await Execute(async () =>
            {
                if (signInManager.IsSignedIn(HttpContext.User))
                {
                    await signInManager.SignOutAsync();
                }
            });
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] AccountCredentialsManifest credentials)
        {
            // Verify parameters
			if (credentials == null || !ModelState.IsValid || credentials.Code == null)
            { return BadRequest(HollowError.MissingInformation.ToString()); }

            return await Execute(async () =>
            {
                var user = await accounts.GetUserAsync(credentials.PhoneNumber);

                if (await userManager.IsLockedOutAsync(user))
				{
					throw new InvalidUserException(HollowError.UserLockedOut.ToString());
                }

                // Check if the account is activated
                if (await userManager.IsPhoneNumberConfirmedAsync(user))
                {
                    // REMOVE FOR PROD
                    credentials.Code = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

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
						throw new InvalidInformationException(HollowError.IncorrectCode.ToString());
					}
                }
                else
                {
                    // REMOVE FOR PROD
                    credentials.Code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

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
                            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                            var confirmationLink = Url.Action("email", "account", new { token, email = user.Email }, Request.Scheme);
                            await emailService.SendEmailAsync(user.Email, "Welcome to Sparrow!", $"Verify your Sparrow email.\n\n{confirmationLink}");
                        }
                    }
                    else
                    {
                        await userManager.AccessFailedAsync(user);
                        throw new InvalidInformationException(HollowError.IncorrectCode.ToString());
                    }
                }
            });
        }

        [HttpGet("email")]
        [AllowAnonymous]
		public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            // Verify parameters
			if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

            return await Execute(async () =>
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new InvalidUserException(HollowError.MissingInformation.ToString());
                }
                // REMOVE FOR PROD
                token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var result = await userManager.ConfirmEmailAsync(user, token);
            });
		}

        [HttpPost("email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailVerification(string email)
        {
            // Verify parameters
            if (string.IsNullOrEmpty(email))
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

            return await Execute(async () =>
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new InvalidUserException(HollowError.MissingInformation.ToString());
                }

                // Send verification email if email is not confirmed
                if (!user.IsEmailConfirmed)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("email", "account", new { token, email = user.Email }, Request.Scheme);
                    await emailService.SendEmailAsync(user.Email, "Verify your Sparrow email.", $"Verify your Sparrow email.\n\n{confirmationLink}");
                }
            });
		}

		[HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] AccountSignUpManifest details)
		{
            // Verify parameters
			if (details == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

            return await Execute(async () =>
            {
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

                    throw e;
                }
            });
        }

        [HttpPut]
        public async Task<IActionResult> ModifyAccount([FromBody] AccountDetailsManifest details)
        {
            // Verify parameters
			if (details == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

            return await Execute(async user =>
            {
                // Send updates to account manager
                await accounts.EditUserAsync(user.Id, name: details.Name);
            }, allowUnverified: true);
        }

		#endregion
	}
}