using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AgriTrace.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            // Register API controllers
            services.AddControllers();

            // Set up Mapster configurations for API
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
