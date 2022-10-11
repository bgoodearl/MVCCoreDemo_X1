using Demo.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDemoInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherForecastService, Services.WeatherForecastService>();
            services.AddSingleton<IBlazorUserService, Services.BlazorUserService>();
            return services;
        }
    }
}
