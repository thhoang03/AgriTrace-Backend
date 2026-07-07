using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid productId);
        Task<IReadOnlyList<Product>> GetAllAsync();
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid productId);
    }
}
