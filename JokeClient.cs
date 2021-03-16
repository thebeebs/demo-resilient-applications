using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;

namespace app
{
    public class JokeClient : IJokeClient
    {
        private readonly PolicyWrap<IEnumerable<Joke>> _policy;

        public JokeClient()
        {
            var timeoutPolicy = Policy
                .TimeoutAsync(2);

            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(5);

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(7, TimeSpan.FromMinutes(1));

            var generalFallback = Policy<IEnumerable<Joke>>
                .Handle<Exception>()
                .FallbackAsync<IEnumerable<Joke>>(
                    async b =>
                    {
                        return Enumerable.Range(1, 5).Select(index => new Joke
                            {
                                SetUp = "generalFallback setup",
                                PunchLine = "generalFallback punchline"
                            })
                            .ToArray();
                    }
                );
            var circuitBreakerFallback = Policy<IEnumerable<Joke>>
                .Handle<BrokenCircuitException>()
                .FallbackAsync<IEnumerable<Joke>>(
                    async b =>
                    {
                        return Enumerable.Range(1, 5).Select(index => new Joke
                            {
                                SetUp = "generalFallback setup",
                                PunchLine = "generalFallback punchline"
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

        public async Task<IEnumerable<Joke>> Jokes()
        {
            var obj = await _policy.ExecuteAsync(async ct =>
            {
                using var httpClient = new HttpClient();
                var response =
                    await httpClient.GetAsync("weatherforecast/Delay?thisshouldrun5times", ct);
                var str = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                return JsonSerializer.Deserialize<IEnumerable<Joke>>(str, options);
            }, CancellationToken.None);

            return obj;
        }
    }
}