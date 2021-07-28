using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparrowServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetEventsList() // Takes in a position model.
        {

            return Ok();
        }

        [HttpGet("{eventId}")]
        public IActionResult GetEvent(uint eventId) // Can also take a model instead of a parameter.
        {

            return Ok();
        }

        [HttpPost]
        public IActionResult CreateEvent() // Takes in an event model.
        {

            return Ok();
        }

        [HttpPut]
        public IActionResult AffectEvent() // Need better name. Takes in an event model and options.
        {

            return Ok();
        }

    }

}
