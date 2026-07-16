using AgriTrace.Domain.Entities;

namespace AgriTrace.Domain.Interfaces.Inbound;


public interface IBatchWriteService
{
    Task<Batch> CreateAsync(
        Batch entity,
        CancellationToken cancellationToken = default);


    Task UpdateAsync(
        Batch entity,
        CancellationToken cancellationToken = default);


    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}