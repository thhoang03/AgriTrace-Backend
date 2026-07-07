using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            var dataModel = await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            return MapToDomain(dataModel);
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            var dataModels = await _dbContext.Products
                .AsNoTracking()
                .ToListAsync();

            return dataModels.Select(MapToDomain).Where(p => p != null).ToList()!;
        }

        public async Task<Product> AddAsync(Product product)
        {
            var dataModel = MapToDataModel(product);
            await _dbContext.Products.AddAsync(dataModel);
            await _dbContext.SaveChangesAsync();
            return MapToDomain(dataModel)!;
        }

        public async Task UpdateAsync(Product product)
        {
            var dataModel = MapToDataModel(product);
            _dbContext.Products.Update(dataModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid productId)
        {
            var dataModel = await _dbContext.Products.FindAsync(productId);
            if (dataModel != null)
            {
                _dbContext.Products.Remove(dataModel);
                await _dbContext.SaveChangesAsync();
            }
        }

        private Product? MapToDomain(ProductDataModel? dataModel)
        {
            if (dataModel == null) return null;
            return new Product(
                dataModel.ProductId,
                dataModel.CategoryId,
                dataModel.Name,
                dataModel.ScientificName,
                dataModel.Description,
                dataModel.ImageUrl,
                dataModel.ShelfLifeDays,
                dataModel.IsActive,
                dataModel.CreatedAt,
                dataModel.UpdatedAt
            );
        }

        private ProductDataModel MapToDataModel(Product domain)
        {
            return new ProductDataModel
            {
                ProductId = domain.ProductId,
                CategoryId = domain.CategoryId,
                Name = domain.Name,
                ScientificName = domain.ScientificName,
                Description = domain.Description,
                ImageUrl = domain.ImageUrl,
                ShelfLifeDays = domain.ShelfLifeDays,
                IsActive = domain.IsActive,
                CreatedAt = domain.CreatedAt,
                UpdatedAt = domain.UpdatedAt
            };
        }
    }
}
