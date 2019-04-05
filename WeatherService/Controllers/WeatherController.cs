using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace WeatherService.Controllers
{
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy<HttpResponseMessage> _resiliencePolicy;

        public WeatherController(HttpClient httpClient, IAsyncPolicy<HttpResponseMessage> resiliencePolicy)
        {
            _httpClient = httpClient;
            _resiliencePolicy = resiliencePolicy;
        }

        [HttpGet("{locationId}")]
        public async Task<ActionResult> Get(int locationId)
        {
            HttpResponseMessage httpResponseMessage = await _resiliencePolicy.ExecuteAsync(() => 
                _httpClient.GetAsync($"temperature/{locationId}"));

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                int temperature = await httpResponseMessage.Content.ReadAsAsync<int>();
                return Ok(temperature);
            }

            return StatusCode((int)httpResponseMessage.StatusCode, "The temperature service returned an error.");
        }
    }
}
