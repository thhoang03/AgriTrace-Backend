using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AgriTrace.Infrastructure.Sqlserver;


public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructureSqlServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {


        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"));
            });



        RegisterRepositories(services);



        return services;
    }



    private static void RegisterRepositories(
        IServiceCollection services)
    {

        // Repository sẽ đăng ký ở đây

        // services.AddScoped<IBatchRepository, BatchRepository>();

    }

}