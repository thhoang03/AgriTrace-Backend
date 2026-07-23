using AgriTrace.Domain.Common;
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
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public sealed class SupplyChainEventReadService : ISupplyChainEventReadService
{
    private readonly ISupplyChainEventReadRepository _repository;

    public SupplyChainEventReadService(ISupplyChainEventReadRepository repository)
    {
        _repository = repository;
    }

    public Task<SupplyChainEvent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<SupplyChainEvent>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    public Task<IReadOnlyList<SupplyChainEvent>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
        => _repository.GetByBatchAsync(batchId, cancellationToken);

    public Task<IReadOnlyList<SupplyChainEvent>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
        => _repository.GetByOrganizationAsync(organizationId, cancellationToken);

    public Task<IReadOnlyList<SupplyChainEvent>> GetByEventTypeAsync(
        Guid eventTypeId,
        CancellationToken cancellationToken = default)
        => _repository.GetByEventTypeAsync(eventTypeId, cancellationToken);

    public Task<PagedResult<SupplyChainEvent>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        => _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
}

