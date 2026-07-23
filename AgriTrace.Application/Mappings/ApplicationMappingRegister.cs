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
                .Map(dest => dest.UnitName, src => src.Unit == null ? null : src.Unit.Name);

            config.NewConfig<Organization, OrganizationDto>()
                .Map(dest => dest.OrganizationTypeName, src => src.OrganizationType == null ? null : src.OrganizationType.Name)
                .Map(dest => dest.OrganizationTypeCode, src => src.OrganizationType == null ? null : src.OrganizationType.Code);
        }
    }
}

