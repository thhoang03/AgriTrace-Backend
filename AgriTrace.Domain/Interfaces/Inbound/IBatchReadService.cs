using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IBatchReadService
{
    Task<Batch?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Batch>> GetAllAsync(
        CancellationToken cancellationToken = default);



    Task<Batch?> GetByBatchCodeAsync(
        string batchCode,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Batch>> GetByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Batch>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Batch>> GetByParentBatchAsync(
        Guid parentBatchId,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyList<Batch>> GetByRootBatchAsync(
        Guid rootBatchId,
        CancellationToken cancellationToken = default);



    Task<PagedResult<Batch>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);



    Task<PagedResult<Batch>> SearchAsync(
        Guid? productId,
        Guid? organizationId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

}