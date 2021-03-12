using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Timeout;
using Polly.Utilities;
using Polly.Wrap;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoWeatherController : ControllerBase
    {
        private WeatherClient weatherClient;

        public DemoWeatherController(WeatherClient weatherClient)
        {
            this.weatherClient = weatherClient;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await this.weatherClient.WeatherForecasts();
        }

       
    }
}
