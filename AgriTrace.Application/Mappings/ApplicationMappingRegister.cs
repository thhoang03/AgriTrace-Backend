using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Entities;
using Mapster;

namespace AgriTrace.Application.Mappings
{
    public class ApplicationMappingRegister : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDto>();
            // Add custom mapping rules here as the application grows.
        }
    }
}
