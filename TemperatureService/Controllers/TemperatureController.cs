using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace TemperatureService.Controllers
{
    [Route("[controller]")]
    public class TemperatureController : ControllerBase
    {
        static int _counter = 0;
        static readonly Random randomTemperature = new Random();

        [HttpGet("{locationId}")]
        public ActionResult Get(int locationId)
        {
            var token = Request.Headers["Token"];
            if (token != "SomeSecret")
            {
                return Unauthorized();
            }

            _counter++;

            if (_counter % 4 == 0) // only one of out four requests will succeed
            {
                return Ok(randomTemperature.Next(0, 120));
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, "Something went wrong when getting the temperature.");
        }
    }
}
