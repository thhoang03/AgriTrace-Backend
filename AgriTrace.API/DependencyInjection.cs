using System.IO;
using System.Reflection;
using System.Text.Json;
using AgriTrace.API.Common;
using AgriTrace.API.Services;
using AgriTrace.API.Swagger;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace AgriTrace.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            // Register API controllers + tự động bọc mọi kết quả vào ApiResponse.
            services.AddControllers(options =>
            {
                options.Filters.Add<ApiResponseWrapperFilter>();
            })
            .AddJsonOptions(o =>
            {
                // Guarantee camelCase JSON for any property without an explicit [JsonPropertyName] (Phase 12).
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // Chuyển mọi exception chưa xử lý thành envelope ApiResponse.
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // Current-user accessor for extracting identity from JWT claims (Phase 10).
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Set up Mapster configurations for API
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            // Register Swashbuckle Swagger (Phase 11).
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AgriTrace API — Agricultural Supply Chain Traceability System",
                    Version = "1.0",
                    Description = "REST API for the Agricultural Supply Chain Traceability System.\n\n" +
                                  "**ID Type Note:** organizationId, categoryId, productId, userId are Guid (UUID) " +
                                  "in this implementation. The swagger.yaml spec declares them as integer. " +
                                  "This is a documented architectural decision to preserve domain integrity.",
                    Contact = new OpenApiContact { Name = "AgriTrace Team" }
                });

                options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter JWT Bearer token"
                });

                // Attach the global Bearer security requirement to the OpenAPI document.
                // A DocumentFilter is used instead of options.AddSecurityRequirement(...)
                // because in Swashbuckle 10.x / Microsoft.OpenApi 2.x the built-in call
                // serialized the requirement as an object instead of an array, so Swagger UI
                // never sent the Authorization header even after clicking "Authorize".
                options.DocumentFilter<BearerSecurityRequirementDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
