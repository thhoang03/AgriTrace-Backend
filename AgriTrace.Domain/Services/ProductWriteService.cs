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
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public sealed class ProductWriteService : IProductWriteService
{
    private readonly IProductWriteRepository _repository;
    private readonly IProductReadRepository _readRepository;

    public ProductWriteService(
        IProductWriteRepository repository,
        IProductReadRepository readRepository)
    {
        _repository = repository;
        _readRepository = readRepository;
    }

    public Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<Product?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
        => _repository.GetByNameAsync(name, cancellationToken);

    public async Task<Product> CreateAsync(
        NewProduct request,
        CancellationToken cancellationToken = default)
    {
        var gtin = request.Gtin?.Trim();

        if (string.IsNullOrWhiteSpace(gtin))
        {
            gtin = await GenerateUniqueGtinAsync(cancellationToken);
        }
        else
        {
            var isGtinExists = await _readRepository.IsGtinExistsAsync(gtin, null, cancellationToken);
            if (isGtinExists)
            {
                throw new InvalidOperationException("GTIN đã được sử dụng cho một sản phẩm khác.");
            }
        }

        var product = new Product(
            request.OrganizationId,
            request.CategoryId,
            request.UnitId,
            request.Name,
            gtin);

        return await _repository.AddAsync(product, cancellationToken);
    }

    public async Task UpdateAsync(
       Guid id,
       UpdateProduct request,
       CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);

        if (product == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        var newGtin = request.Gtin?.Trim();
        var currentGtin = product.Gtin;

        if (newGtin != currentGtin)
        {
            // Only block changing GTIN if the product ALREADY had a GTIN and has batches.
            // If the product previously had NO GTIN (currentGtin is null/empty), allow assigning one for the first time.
            if (!string.IsNullOrWhiteSpace(currentGtin))
            {
                var hasBatches = await _readRepository.HasBatchesAsync(id, cancellationToken);
                if (hasBatches)
                {
                    throw new InvalidOperationException("Không thể thay đổi GTIN vì sản phẩm đã được sử dụng trong các lô hàng.");
                }
            }

            if (!string.IsNullOrWhiteSpace(newGtin))
            {
                var isGtinExists = await _readRepository.IsGtinExistsAsync(newGtin, id, cancellationToken);
                if (isGtinExists)
                {
                    throw new InvalidOperationException("GTIN đã được sử dụng cho một sản phẩm khác.");
                }
            }
        }

        product.UpdateInformation(
            request.CategoryId,
            request.UnitId,
            request.Name,
            request.Gtin);

        await _repository.UpdateAsync(
            product,
            cancellationToken);
    }

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);

    private async Task<string> GenerateUniqueGtinAsync(CancellationToken cancellationToken)
    {
        var random = new Random();
        for (int attempt = 0; attempt < 50; attempt++)
        {
            // 893 (VN GS1 Prefix) + 8 random digits = 11 digits
            var digits = "893" + random.Next(10000000, 99999999).ToString("D8");
            
            // Calculate GS1 Modulo 10 Checksum
            int sum = 0;
            for (int i = 0; i < 11; i++)
            {
                int val = digits[i] - '0';
                sum += (i % 2 == 0) ? val : val * 3;
            }
            int checkDigit = (10 - (sum % 10)) % 10;
            var candidateGtin = digits + checkDigit;

            var exists = await _readRepository.IsGtinExistsAsync(candidateGtin, null, cancellationToken);
            if (!exists)
            {
                return candidateGtin;
            }
        }

        // Fallback with timestamp-based digits
        var fallbackDigits = "893" + (DateTime.UtcNow.Ticks % 100000000).ToString("D8");
        int fallbackSum = 0;
        for (int i = 0; i < 11; i++)
        {
            int val = fallbackDigits[i] - '0';
            fallbackSum += (i % 2 == 0) ? val : val * 3;
        }
        int fallbackCheck = (10 - (fallbackSum % 10)) % 10;
        return fallbackDigits + fallbackCheck;
    }
}
