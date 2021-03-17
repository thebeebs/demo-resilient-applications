using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace app
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
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_jokeApi);
            var str = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
            return JsonSerializer.Deserialize<IEnumerable<Joke>>(str, options);
        }
    }
}