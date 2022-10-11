using Demo.Shared.Models;
using System;
using System.Threading.Tasks;

namespace Demo.Shared.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
    }
}
