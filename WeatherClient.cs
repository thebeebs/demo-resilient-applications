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
    public class WeatherClient
    {
        private readonly PolicyWrap<IEnumerable<WeatherForecast>> _policy;

        public WeatherClient()
        {
            var timeoutPolicy = Policy
                .TimeoutAsync(2);
                
            var retryPolicy= Policy
                .Handle<Exception>()
                .RetryAsync(5);
            
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(7, TimeSpan.FromMinutes(1));

            var generalFallback = Policy<IEnumerable<WeatherForecast>>
                .Handle<Exception>()
                .FallbackAsync<IEnumerable<WeatherForecast>>(
                    async b =>
                    {
                        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                            {
                                Date = DateTime.Now,
                                TemperatureC = 0,
                                Summary = "This comes from a fallback"
                            })
                            .ToArray();
                    }
                );
            var circuitBreakerFallback = Policy<IEnumerable<WeatherForecast>>
                .Handle<BrokenCircuitException>()
                .FallbackAsync<IEnumerable<WeatherForecast>>(
                    async b =>
                    {
                        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                            {
                                Date = DateTime.Now,
                                TemperatureC = 0,
                                Summary = "This comes from a fallback because of a circuit breaker"
                            })
                            .ToArray();
                    }
                );
            _policy = generalFallback
                .WrapAsync(circuitBreakerFallback)
                .WrapAsync(retryPolicy)
                .WrapAsync(timeoutPolicy)
                .WrapAsync(circuitBreakerPolicy);
        }
        
        public async Task<IEnumerable<WeatherForecast>> WeatherForecasts()
        {
            var obj = await _policy.ExecuteAsync(async (ct) =>
            {
                using var httpClient = new HttpClient();
                var response =
                    await httpClient.GetAsync("http://localhost:5000/api/weatherforecast/Delay?thisshouldrun5times", ct);
                var str = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(str, options);
            }, CancellationToken.None);

            return obj;
        }

    }
}