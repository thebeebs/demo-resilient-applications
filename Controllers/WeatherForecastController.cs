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
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        public enum Failure
        {   
            Delay,
            RandomDelay,
            None,
        }
        private readonly TimeSpan delay = TimeSpan.FromSeconds(20);
        
        [HttpGet]
        public  IEnumerable<WeatherForecast> Get(Failure failure = Failure.None) => failure switch
        {
            Failure.Delay => DelayWeather(),
            Failure.RandomDelay => DelayWeather(new Random().Next(0,20)),
            Failure.None => CurrentWeather(),
            _ => null
        };


        [NonAction]  
        public IEnumerable<WeatherForecast> DelayWeather(int secondsToDelay = 20){
            Task.Delay(TimeSpan.FromSeconds(secondsToDelay)).Wait();
            return CurrentWeather();
        }

        [NonAction]  
        public IEnumerable<WeatherForecast> CurrentWeather(){
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
