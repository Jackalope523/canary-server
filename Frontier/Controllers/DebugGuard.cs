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
				List<EventShard> seedEvents = new();

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

                log.LogError("Creating events..");

                foreach (var @event in seed.Events)
				{
					var host = await accounts.GetCoreUserAsync(seedUsers[(int) @event.Host.Id - 1].PhoneNumber);

                    // Move host to proposed event location
                    // await accounts.UpdateUserLocationAsync(host.Id, @event.Latitude, @event.Longitude);
					
					seedEvents.Add(await events.CreateEventAsync(host.Id,
						@event.Name, @event.Description,
						@event.StartTime, @event.Latitude, @event.Longitude,
						@event.Radius, @event.IsDynamic,
						@event.GroupMinimum, @event.GroupMaximum));
				}

                log.LogError("Joining users to events..");

                for (int i = 0; i < seed.Attendance.Count; i++)
				{
					var tuple = seed.Attendance[i];
					var self = tuple[0]; var @event = tuple[1];

					// Move user to event location
					await accounts.UpdateUserLocationAsync(seedUsers[self - 1].Id, seedEvents[@event - 1].Latitude, seedEvents[@event - 1].Longitude);

					// Attempt to join the event
					await events.JoinEventAsync(seedUsers[self - 1].Id, seedEvents[@event - 1].Id);
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