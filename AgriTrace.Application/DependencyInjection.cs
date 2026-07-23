using System.Reflection;
using AgriTrace.Application.Common.Behaviors;
using AgriTrace.Application.Services;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AgriTrace.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register FluentValidation pipeline behavior (runs validators before every handler)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Auto-register all validators in this assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register Mapster Config
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            // Register Domain Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductReadService, ProductReadService>();
            services.AddScoped<IProductWriteService, ProductWriteService>();
            services.AddScoped<IBatchReadService, BatchReadService>();
            services.AddScoped<IBatchWriteService, BatchWriteService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IQualityInspectionService, QualityInspectionService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ISupplyChainEventWriteService, SupplyChainEventWriteService>();
            services.AddScoped<IHashChainService, HashChainService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();

            // Phase 8: events, split/merge, recalls, notifications
            services.AddScoped<IEventTypeService, EventTypeService>();
            services.AddScoped<IBatchSplitService, BatchSplitService>();
            services.AddScoped<IBatchMergeService, BatchMergeService>();
            services.AddScoped<IRecallService, RecallService>();
            services.AddScoped<INotificationService, NotificationService>();

            // Phase 9: public trace, analytics, lookup
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IOrganizationTypeService, OrganizationTypeService>();

            return services;
        }
    }
}
