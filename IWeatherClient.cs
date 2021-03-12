using System.Collections.Generic;
using System.Threading.Tasks;

namespace app
{
    public interface IWeatherClient
    {
        Task<IEnumerable<WeatherForecast>> WeatherForecasts();
    }
}