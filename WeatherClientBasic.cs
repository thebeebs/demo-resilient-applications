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

namespace app
{
    public class WeatherClientBasic : IWeatherClient
    {
        private readonly PolicyWrap<IEnumerable<WeatherForecast>> _policy;

        public WeatherClientBasic()
        {
        }
        
        public async Task<IEnumerable<WeatherForecast>> WeatherForecasts()
        {
            using var httpClient = new HttpClient();
                var response =
                    await httpClient.GetAsync("api/weatherforecast");
                var str = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(str, options);
            
        }

    }
}