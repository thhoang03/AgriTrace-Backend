using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
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
