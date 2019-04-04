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
            _counter++;

            var token = Request.Headers["Token"];

            if ((_counter == 1 && token == "OldPassword") || (_counter >= 2 && token == "NewPassword"))
            {
                return Ok(randomTemperature.Next(0, 120));
            }
            return Unauthorized();


        }
    }
}
