using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Boundaries;

namespace Frontier.Controllers
{
    [Route("discover")]
    public class DiscoverAgent : AbstractAgent
	{
		#region Initialisation

		public DiscoverAgent(UserManager<UserShard> identityUserManager, SignInManager<UserShard> identitySignInManager,
			IAccountOperations accountOperations, IProfileOperations profileOperations,
			IEventOperations eventOperations, IEtchingOperations etchingOperations,
			IReportOperations reportOperations, INotificationOperations notificationOperations,
			ISMSService externalSMSService, IEmailService externalEmailService) :
			base(identityUserManager, identitySignInManager,
				accountOperations, profileOperations,
				eventOperations, etchingOperations,
				reportOperations, notificationOperations,
				externalSMSService, externalEmailService)
		{ }

		#endregion

		#region Actions

		[HttpGet("{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetEvents(float latitude, float longitude, float distance)
        {
            List<EventThinSlice> eventList;

            try
            {
                // Retrieve events personalised for the current user
                var user = await GetCurrentUserAsync();
                eventList = await events.GetPersonalisedEventsInAreaAsync(user.Id, latitude, longitude, distance);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventList);
        }

        [HttpGet("all/{latitude}-{longitude}-{distance}")]
        public async Task<IActionResult> GetAllEvents(float latitude, float longitude, float distance)
        {
            List<EventThinSlice> eventList;

            try
			{
                // Retrieve all events available to the current user
				var user = await GetCurrentUserAsync();
				eventList = await events.GetEventsInAreaAsync(user.Id, latitude, longitude, distance);
			}
			catch (Exception e)
			{
				return BadRequest(e.ToString());
			}

			return Ok(eventList);
        }

        [HttpPost("user/{latitude}-{longitude}")]
        public async Task<IActionResult> UpdateCurrentPosition(float latitude, float longitude)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                await accounts.UpdateUserLocationAsync(user.Id, latitude, longitude);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Ok();
        }

		#endregion
	}
}