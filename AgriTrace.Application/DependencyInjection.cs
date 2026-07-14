using System.Reflection;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AgriTrace.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register Mapster Config
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            // Register Domain Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductReadService, ProductReadService>();
            services.AddScoped<IProductWriteService, ProductWriteService>();

            return services;
        }
    }
}
