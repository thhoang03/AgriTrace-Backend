using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using AgriTrace.Infrastructure.Sqlserver.Repositories;
using AgriTrace.Infrastructure.Sqlserver.Repositories.Read;
using AgriTrace.Infrastructure.Sqlserver.Repositories.Write;
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
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IQualityInspectionRepository, QualityInspectionRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<IBatchReadRepository, BatchReadRepository>();
        services.AddScoped<IBatchWriteRepository, BatchWriteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IOrganizationTypeRepository, OrganizationTypeRepository>();

        // Phase 8: events, split/merge, recalls, notifications
        services.AddScoped<IEventTypeRepository, EventTypeRepository>();
        services.AddScoped<ISupplyChainEventRepository, SupplyChainEventRepository>();
        services.AddScoped<IEventRepository, SupplyChainEventRepository>();
        services.AddScoped<IBatchSplitRepository, BatchSplitRepository>();
        services.AddScoped<IBatchMergeRepository, BatchMergeRepository>();
        services.AddScoped<IRecallRepository, RecallRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }

}