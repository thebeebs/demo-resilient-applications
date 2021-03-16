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
            new Tuple<string, string>("There are 10 types of people in the world.","Those who understand binary, and those who don't"), 
            new Tuple<string, string>("How many programmers does it take to change a light bulb?","None. It's a hardware problem."),
            new Tuple<string, string>("In order to understand recursion.","You must first understand recursion."),
            new Tuple<string, string>("I would tell you a UDP joke.","But you might not get it."),
            new Tuple<string, string>("There are only two hard things in computer science.","Cache invalidation, naming things, and off-by-one errors."),
            new Tuple<string, string>("What do computers eat for a snack?","Microchips!"),
            new Tuple<string, string>("Whats the object-oriented way to become wealthy?","Inheritance."),
            new Tuple<string, string>("Why did Bilbo Baggins always eat his lunch at noon?","He was always a creature of Hobbit."),
            new Tuple<string, string>("Why are monkeys great at sports?","They are born chimpions."),
            new Tuple<string, string>("Why don't werewolves ever know the time?","Because they're not whenwolves."),
            new Tuple<string, string>("Why did the chicken cross the road?","That's really only the chicken's business."),
            new Tuple<string, string>("Do you want to hear a pizza joke?","Never mind, it is pretty cheesy."),
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
                    SetUp = Jokes[item].Item1,
                    PunchLine = Jokes[item].Item2,
                    Source = "Third Party API"
                })
                .ToArray();
        }
    }
}