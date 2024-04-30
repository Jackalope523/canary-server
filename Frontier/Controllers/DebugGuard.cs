using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;
using Microsoft.AspNetCore.Authorization;

namespace Frontier.Controllers
{
	[Route("debug")]
	public class DebugGuard : AbstractGuard
	{
		#region Initialisation

		private IDebugOperations debug;

		public DebugGuard(GuardBox box, UserManager<UserShard> aspUserManager, IDebugOperations debugOperations) :
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
				await debug.SeedDatabaseAsync();

				List<UserShard> seedUsers = new();
				List<EventShard> seedEvents = new();
			
				foreach (var user in seed.Users)
				{
					await debug.AddUserToBannerAsync(user.PhoneNumber, "debug");
					await accounts.CreateUserAsync(user.PhoneNumber, user.Email, user.Name, user.DateOfBirth);

					var userShard = await accounts.GetUserAsync(user.PhoneNumber);

                    seedUsers.Add(userShard);

					// Confirm user phone
					var code = await userManager.GenerateChangePhoneNumberTokenAsync(userShard, user.PhoneNumber);
					await userManager.VerifyTwoFactorTokenAsync(userShard, TokenOptions.DefaultPhoneProvider, code);

					// Confirm user email
					var token = await userManager.GenerateEmailConfirmationTokenAsync(userShard);
					await userManager.ConfirmEmailAsync(userShard, token);
				}

				foreach (var @event in seed.Events)
				{
					var host = await accounts.GetUserAsync(seedUsers[(int) @event.Host.Id - 1].PhoneNumber);

                    // Move host to proposed event location
                    // await accounts.UpdateUserLocationAsync(host.Id, @event.Latitude, @event.Longitude);

					seedEvents.Add(await events.CreateEventAsync(host.Id,
						@event.Name, @event.Description,
						@event.StartTime, @event.Latitude, @event.Longitude,
						@event.Radius, @event.IsDynamic,
						@event.GroupMinimum, @event.GroupMaximum));
				}

				for (int i = 0; i < seed.Attendance.Count; i++)
				{
					var tuple = seed.Attendance[i];
					var self = tuple[0]; var @event = tuple[1];

					// Move user to event location
					await accounts.UpdateUserLocationAsync(seedUsers[self - 1].Id, seedEvents[@event - 1].Latitude, seedEvents[@event - 1].Longitude);

					// Attempt to join the event
					await events.JoinEventAsync(seedUsers[self - 1].Id, seedEvents[@event - 1].Id);
				}

				for (int i = 0; i < seed.Follows.Count; i++)
				{
					var tuple = seed.Follows[i];
					var self = tuple[0]; var other = tuple[1];

					await profiles.FollowUserAsync(seedUsers[self - 1].Id, seedUsers[other - 1].Id);
				}

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