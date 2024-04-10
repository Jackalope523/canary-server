using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;
using Microsoft.Extensions.Logging;
using Frontier.Manifests;

namespace Frontier.Controllers
{
    [Route("discover")]
    public class DiscoverGuard : AbstractGuard
	{
		#region Initialisation

		public DiscoverGuard(ILogger logger,
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

		[HttpGet("{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetEvents(float latitude, float longitude, float distance)
        {
			return await Execute(async user =>
			{
				// Retrieve events personalised for the current user
				List<EventManifest> eventList = (await events.GetPersonalisedEventsInAreaAsync(user.Id, latitude, longitude, distance))
					.ConvertAll(shard => new EventManifest(shard));

				return eventList;
			});
        }

        [HttpGet("all/{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetAllEvents(float latitude, float longitude, float distance)
        {
			return await Execute(async user =>
			{
                // Retrieve all events available to the current user
                List<EventManifest> eventList = (await events.GetEventsInAreaAsync(user.Id, latitude, longitude, distance))
					.ConvertAll(shard => new EventManifest(shard));

                return eventList;
			});
        }

        [HttpPost("user/{latitude}-{longitude}")]
        public async Task<IActionResult> UpdateCurrentPosition(float latitude, float longitude)
        {
			return await Execute(async user =>
			{
				await accounts.UpdateUserLocationAsync(user.Id, latitude, longitude);
			}, allowUnverified: true);
        }

		#endregion
	}
}