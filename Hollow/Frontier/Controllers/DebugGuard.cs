using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Frontier.Manifests;
using Core.Boundaries;

namespace Frontier.Controllers
{
	[Route("debug")]
	public class DebugGuard : AbstractGuard
	{
		#region Initialisation

		private IDebugOperations debug;

		public DebugGuard(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IDisciplineOperations disciplineOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService, IDebugOperations debugOperations) :
			base(identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				disciplineOperations, notificationOperations,
				externalSMSService, externalEmailService)
		{
			debug = debugOperations;
		}

		#endregion

		#region Actions

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
					await accounts.CreateUserAsync(user.PhoneNumber, user.Email, user.Name, user.DateOfBirth);

					seedUsers.Add(await accounts.GetUserAsync(user.PhoneNumber));
				}

				foreach (var @event in seed.Events)
				{
					seedEvents.Add(await events.CreateEventAsync(@event.Host.Id,
						@event.EventName, @event.EventDescription,
						@event.StartTime, @event.Latitude, @event.Longitude,
						@event.Radius, @event.IsDynamic,
						@event.GroupMinimum, @event.GroupMaximum));
				}

				foreach ((var self, var @event) in seed.Attendance)
				{
					await events.JoinEventAsync(seedUsers[self - 1].Id, seedEvents[@event - 1].Id);
				}

				foreach ((var self, var other) in seed.Follows)
				{
					await profiles.FollowUserAsync(seedUsers[self - 1].Id, seedUsers[other - 1].Id);
				}

				foreach ((var self, var other) in seed.Blocks)
				{
					await profiles.BlockUserAsync(seedUsers[self - 1].Id, seedUsers[other - 1].Id);
				}
			});
		}

		#endregion
	}
}