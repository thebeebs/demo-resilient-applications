using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]/{failure?}")]
    public class ThirdPartyJokeController : ControllerBase
    {
        public enum Failure
        {
            Delay,
            RandomDelay,
            None
        }

        private static readonly Tuple<string, string>[] Jokes =
        {
            new Tuple<string, string>("s1","p1"), 
            new Tuple<string, string>("s2","p2"),
            new Tuple<string, string>("s3","p3"),
            new Tuple<string, string>("s4","p4"),
            new Tuple<string, string>("s5","p5"),
        };
        

        private readonly ILogger<ThirdPartyJokeController> _logger;

        public ThirdPartyJokeController(ILogger<ThirdPartyJokeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Joke> Get(Failure failure = Failure.None)
        {
            return failure switch
            {
                Failure.Delay => DelayJoke(),
                Failure.RandomDelay => DelayJoke(new Random().Next(0, 10)),
                Failure.None => RandomJoke(),
                _ => null
            };
        }


        [NonAction]
        public IEnumerable<Joke> DelayJoke(int secondsToDelay = 5)
        {
            Task.Delay(TimeSpan.FromSeconds(secondsToDelay)).Wait();
            return RandomJoke();
        }

        [NonAction]
        public IEnumerable<Joke> RandomJoke()
        {
            var rng = new Random();
            var item = rng.Next(Jokes.Length);
            return Enumerable.Range(1, 1).Select(index => new Joke
                {
                    PunchLine = Jokes[item].Item1,
                    SetUp = Jokes[item].Item2,
                })
                .ToArray();
        }
    }
}