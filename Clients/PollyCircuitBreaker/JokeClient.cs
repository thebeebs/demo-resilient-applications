using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;

namespace app.Clients.PollyCircuitBreaker
{
    public class JokeClient : IJokeClient
    {
        private readonly string _jokeApi;
        private PolicyWrap<IEnumerable<Joke>> _policyWrap;

        public JokeClient(IConfiguration configuration)
        {
            _jokeApi = configuration["JOKEURL"];
            var policy = Policy.TimeoutAsync(2);
            
            var generalFallback = Policy<IEnumerable<Joke>>
                .Handle<Exception>()
                .FallbackAsync<IEnumerable<Joke>>(
                    async (ct) => new List<Joke>
                    {
                        new Joke
                        {
                            SetUp = "I am in a band called 1023MB.",
                            PunchLine = "We haven't had any gigs yet.",
                            Source = "Fallback"
                        }
                    });

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, (retryAttempt) =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMilliseconds(new Random().Next(0,1000))
                );

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));
            
            var circuitBreakerFallback = Policy<IEnumerable<Joke>>
                .Handle<BrokenCircuitException>()
                .FallbackAsync<IEnumerable<Joke>>(
                    async (ct) => new List<Joke>
                    {
                        new Joke
                        {
                            SetUp = "Why did the developer go broke?",
                            PunchLine = "Because they used up all their cache.",
                            Source = "Circuit Breaker Fallback"
                        }
                    });

            _policyWrap = generalFallback
                .WrapAsync(retryPolicy)
                .WrapAsync(circuitBreakerFallback)
                .WrapAsync(circuitBreakerPolicy)
                .WrapAsync(policy);
        }

        public async Task<IEnumerable<Joke>> Jokes()
        {
            return await _policyWrap.ExecuteAsync(async ct =>
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(_jokeApi, ct);
                var str = await response.Content.ReadAsStringAsync(ct);
                var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
                return JsonSerializer.Deserialize<IEnumerable<Joke>>(str, options);
            }, new CancellationToken());
        }
    }
}