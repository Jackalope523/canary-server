using Microsoft.AspNetCore.Mvc;
using Server.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Utilities;

namespace Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiscoverController : Controller
    {
        enum DiscoverError
        {
            MissingInformation,
            CouldNotFindEvent,
            CouldNotCompleteRequest
        }

        IEventOperations events;

        public DiscoverController(IEventOperations eventOperations)
        {
            events = eventOperations;
        }

        [HttpGet]
        public IActionResult GetEvents([FromBody] GeoLocation userLocation)
        {
            if (userLocation == null || !ModelState.IsValid)
            {
                return BadRequest(DiscoverError.MissingInformation.ToString());
            }

            List<ThinnerEvent> eventList;

            try
            {
                eventList = events.GetPersonalisedEventsInArea(userLocation.UserID, userLocation.Latitude, userLocation.Longitude, userLocation.Distance);
            }
            catch
            {
                return BadRequest(DiscoverError.CouldNotCompleteRequest.ToString());
            }

            return Ok(eventList);
        }

        [HttpGet("all")]
        public IActionResult GetAllEvents([FromBody] GeoLocation userLocation)
        {
            if (userLocation == null || !ModelState.IsValid)
            {
                return BadRequest(DiscoverError.MissingInformation.ToString());
            }

            List<ThinnerEvent> eventList;

            try
            {
                eventList = events.GetEventsInArea(userLocation.UserID, userLocation.Latitude, userLocation.Longitude, userLocation.Distance);
            }
            catch
            {
                return BadRequest(DiscoverError.CouldNotCompleteRequest.ToString());
            }

            return Ok(eventList);
        }

        [HttpGet("{latitude}-{longitude}")]
        public IActionResult GetMapFiles(uint latitude, float longitude)
        {
            return Ok();
        }

    }

}
