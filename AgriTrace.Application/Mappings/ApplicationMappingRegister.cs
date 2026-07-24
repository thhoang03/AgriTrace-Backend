using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using Mapster;

namespace AgriTrace.Application.Mappings
{
    public class ApplicationMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.CategoryName, src => src.Category == null ? null : src.Category.Name)
                .Map(dest => dest.UnitName, src => src.Unit == null ? null : src.Unit.Name)
                .Map(dest => dest.OrganizationName, src => src.Organization == null ? null : src.Organization.Name);

            config.NewConfig<Organization, OrganizationDto>()
                .Map(dest => dest.OrganizationTypeName, src => src.OrganizationType == null ? null : src.OrganizationType.Name)
                .Map(dest => dest.OrganizationTypeCode, src => src.OrganizationType == null ? null : src.OrganizationType.Code);

            config.NewConfig<Batch, BatchDto>()
                .Map(dest => dest.ProductName, src => src.Product == null ? null : src.Product.Name)
                .Map(dest => dest.CategoryId, src => src.Product == null ? null : src.Product.CategoryId)
                .Map(dest => dest.CategoryName, src => src.Product == null || src.Product.Category == null ? null : src.Product.Category.Name)
                .Map(dest => dest.UnitCode, src => src.Unit == null ? null : src.Unit.Code)
                .Map(dest => dest.OrganizationName, src => src.CurrentOrganization != null ? src.CurrentOrganization.Name : (src.Product != null && src.Product.Organization != null ? src.Product.Organization.Name : null));
        }
    }
}

