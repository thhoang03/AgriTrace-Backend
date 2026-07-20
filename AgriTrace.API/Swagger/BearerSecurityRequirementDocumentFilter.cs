using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AgriTrace.API.Swagger;

/// <summary>
/// Adds a global Bearer security requirement to every operation in the OpenAPI
/// document. This is required because <c>AddSecurityRequirement</c> in
/// Swashbuckle 10.x / Microsoft.OpenApi 2.x does not reliably attach the
/// scheme reference to each path, which caused Swagger UI to omit the
/// <c>Authorization</c> header even after clicking "Authorize".
/// </summary>
public sealed class BearerSecurityRequirementDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        var requirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("BearerAuth", document, null),
                new List<string>()
            }
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(requirement);
    }
}
