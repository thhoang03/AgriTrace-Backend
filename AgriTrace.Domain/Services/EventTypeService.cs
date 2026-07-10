using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class EventTypeService : IEventTypeService
{

    private readonly IEventTypeRepository _repository;


    public EventTypeService(
        IEventTypeRepository repository)
    {
        _repository = repository;
    }



    public async Task<EventType> CreateAsync(
        EventType entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(
            entity,
            cancellationToken);
    }



    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(
            id,
            cancellationToken);
    }



    public async Task<IReadOnlyList<EventType>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }



    public async Task<EventType?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByCodeAsync(
            code,
            cancellationToken);
    }



    public async Task<EventType?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }



    public async Task<PagedResult<EventType>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);
    }



    public async Task UpdateAsync(
        EventType entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }
}