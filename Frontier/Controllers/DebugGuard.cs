using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Frontier.Controllers
{
	[Route("debug")]
	public class DebugGuard : AbstractGuard
	{
		#region Initialisation

		private IDebugOperations debug;

		public DebugGuard(GuardBox box, UserManager<CoreUser> aspUserManager, IDebugOperations debugOperations) :
			base(box, aspUserManager)
		{
			debug = debugOperations;
		}

		#endregion

		#region Actions

		[AllowAnonymous]
		[HttpPost("seed")]
		public async Task<IActionResult> Seed([FromBody] SeedManifest seed)
		{
			// Verify parameters
			if (seed == null || !ModelState.IsValid)
			{ return BadRequest(HollowError.MissingInformation.ToString()); }

			return await Execute(async () =>
			{
				log.LogError("Begining database seeding.\nDraining the database.");

				await debug.SeedDatabaseAsync();

				List<CoreUser> seedUsers = new();
				List<GatheringShard> seedGatherings = new();

                log.LogError("Adding users..");

                foreach (var user in seed.Users)
				{
					await debug.AddUserToBannerAsync(user.PhoneNumber, "debug");
					await accounts.CreateUserAsync(user.PhoneNumber, user.Email, user.Name, user.DateOfBirth);

					var coreUser = await accounts.GetCoreUserAsync(user.PhoneNumber);

                    seedUsers.Add(coreUser);

					// Confirm user phone
					var code = await userManager.GenerateChangePhoneNumberTokenAsync(coreUser, user.PhoneNumber);
					await userManager.VerifyTwoFactorTokenAsync(coreUser, TokenOptions.DefaultPhoneProvider, code);

					// Confirm user email
					var token = await userManager.GenerateEmailConfirmationTokenAsync(coreUser);
					await userManager.ConfirmEmailAsync(coreUser, token);
				}

                log.LogError("Creating gatherings..");

                foreach (var @gathering in seed.Gatherings)
				{
					var host = await accounts.GetCoreUserAsync(seedUsers[(int) @gathering.Host.Id - 1].PhoneNumber);

                    // Move host to proposed gathering location
                    // await accounts.UpdateUserLocationAsync(host.Id, @gathering.Latitude, @gathering.Longitude);
					
					seedGatherings.Add(await gatherings.CreateGatheringAsync(host.Id,
						@gathering.Name, @gathering.Description,
						@gathering.StartTime, @gathering.Latitude, @gathering.Longitude,
						@gathering.Radius, @gathering.IsDynamic,
						@gathering.GroupMinimum, @gathering.GroupMaximum));
				}

                log.LogError("Joining users to gatherings..");

                for (int i = 0; i < seed.Attendance.Count; i++)
				{
					var tuple = seed.Attendance[i];
					var self = tuple[0]; var @gathering = tuple[1];

					// Move user to gathering location
					await accounts.UpdateUserLocationAsync(seedUsers[self - 1].Id, seedGatherings[@gathering - 1].Latitude, seedGatherings[@gathering - 1].Longitude);

					// Attempt to join the gathering
					await gatherings.JoinGatheringAsync(seedUsers[self - 1].Id, seedGatherings[@gathering - 1].Id);
				}

                log.LogError("Making friends..");

                for (int i = 0; i < seed.Follows.Count; i++)
				{
					var tuple = seed.Follows[i];
					var self = tuple[0]; var other = tuple[1];

					await profiles.FollowUserAsync(seedUsers[self - 1].Id, seedUsers[other - 1].Id);
				}

                log.LogError("Making enemies..");

                for (int i = 0; i < seed.Blocks.Count; i++)
                {
                    var tuple = seed.Blocks[i];
                    var self = tuple[0]; var other = tuple[1];

                    await profiles.BlockUserAsync(seedUsers[self - 1].Id, seedUsers[other - 1].Id);
				}
			});
		}

		#endregion
	}
}