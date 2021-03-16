using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Polly;

namespace app.Clients.PollyFallback
{
    public class JokeClient : IJokeClient
    {
        private readonly string _jokeApi;

        public JokeClient(IConfiguration configuration)
        {
            _jokeApi = configuration["JOKEURL"];
        }

        public async Task<IEnumerable<Joke>> Jokes()
        {
            var policy = Policy.TimeoutAsync(5);
            
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

            var policyWrap = generalFallback.WrapAsync(policy);
            
            return await policyWrap.ExecuteAsync(async ct =>
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