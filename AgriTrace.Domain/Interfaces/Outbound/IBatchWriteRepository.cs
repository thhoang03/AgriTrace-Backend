using AgriTrace.Domain.Entities;


namespace AgriTrace.Domain.Interfaces.Outbound;


public interface IBatchWriteRepository
{

    Task<Batch> AddAsync(
        Batch entity,
        CancellationToken cancellationToken = default);



    Task UpdateAsync(
        Batch entity,
        CancellationToken cancellationToken = default);



    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

}