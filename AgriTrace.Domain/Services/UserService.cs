using AgriTrace.Domain.Common;
using AgriTrace.Domain.Common.Enums;
using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Interfaces.Inbound;
using AgriTrace.Domain.Interfaces.Outbound;


namespace AgriTrace.Domain.Services;


public class UserService : IUserService
{

    private readonly IUserRepository _repository;



    public UserService(
        IUserRepository repository)
    {
        _repository = repository;
    }



    public async Task<User> CreateAsync(
        User entity,
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




    public async Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(
            cancellationToken);
    }




    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }




    public async Task<PagedResult<User>> GetPagedAsync(
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
        User entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(
            entity,
            cancellationToken);
    }





    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByEmailAsync(
            email,
            cancellationToken);
    }





    public async Task<IReadOnlyList<User>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByOrganizationAsync(
            organizationId,
            cancellationToken);
    }




    public async Task<IReadOnlyList<User>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByRoleAsync(
            role,
            cancellationToken);
    }



    public async Task<User?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByRefreshTokenAsync(
            refreshToken,
            cancellationToken);
    }



    public async Task<User?> GetByResetTokenAsync(
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByResetTokenAsync(
            resetToken,
            cancellationToken);
    }
}
