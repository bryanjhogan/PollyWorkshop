using System;
using Microsoft.AspNetCore.Mvc;

namespace TemperatureService.Controllers
{
    [Route("Humidity")]
    public class HumidityController : Controller
    {
        static readonly Random randomHumidity = new Random();

        [HttpGet("{locationId}")]
        public ActionResult Get(int locationId)
        {

            return Ok(randomHumidity.Next(0, 100));
        }
    }
}