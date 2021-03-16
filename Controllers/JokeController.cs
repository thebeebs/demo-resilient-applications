using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JokeController : ControllerBase
    {
        private readonly IJokeClient _jokeClient;

        public JokeController(IJokeClient jokeClient)
        {
            _jokeClient = jokeClient;
        }

        [HttpGet]
        public async Task<IEnumerable<Joke>> Get()
        {
            return await _jokeClient.Jokes();
        }
    }
}