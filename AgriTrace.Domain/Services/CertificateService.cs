using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;

namespace AgriTrace.Domain.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;

    public CertificateService(
        ICertificateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Certificate> CreateAsync(
        Certificate entity,
        CancellationToken cancellationToken = default)
    {
        return await _repository.AddAsync(
            entity,
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(
            id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Certificate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }

    public async Task<Certificate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Certificate>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByBatchAsync(
            batchId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Certificate>> GetByInspectionAsync(
        Guid inspectionId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByInspectionAsync(
            inspectionId,
            cancellationToken);
    }

    public async Task<PagedResult<Certificate>> GetPagedAsync(
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
        Certificate entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }
}