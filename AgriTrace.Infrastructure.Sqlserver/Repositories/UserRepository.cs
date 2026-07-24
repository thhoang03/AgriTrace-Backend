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
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;


namespace AgriTrace.Infrastructure.Sqlserver.Repositories;


public class UserRepository
    : IUserRepository
{

    private readonly ApplicationDbContext _context;



    public UserRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }





    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Users
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Users
            .Include(x => x.Organization)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<PagedResult<User>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Include(x => x.Organization)
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var entities = models
            .Select(ToEntity)
            .ToList();

        return new PagedResult<User>(
            entities,
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<User> AddAsync(
        User entity,
        CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await _context.Users
            .AddAsync(
                model,
                cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        User entity,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == entity.Id,
                cancellationToken);

        if (model == null)
            return;

        model.OrganizationId = entity.OrganizationId;
        model.Email = entity.Email;
        model.FullName = entity.FullName;
        model.Phone = entity.Phone;
        model.PasswordHash = entity.PasswordHash;
        model.Role = entity.Role;
        model.IsActive = entity.IsActive;
        model.RefreshToken = entity.RefreshToken;
        model.RefreshTokenExpiry = entity.RefreshTokenExpiry;
        model.ResetPasswordToken = entity.ResetPasswordToken;
        model.ResetPasswordTokenExpiry = entity.ResetPasswordTokenExpiry;
        model.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (model == null)
            return;

        _context.Users.Remove(model);

        await _context.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Users
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<IReadOnlyList<User>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Users
            .Include(x => x.Organization)
            .Where(
                x => x.OrganizationId == organizationId)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<IReadOnlyList<User>> GetByRoleAsync(
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.Users
            .Include(x => x.Organization)
            .Where(
                x => x.Role == role)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models
            .Select(ToEntity)
            .ToList();
    }

    public async Task<User?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Users
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(
                x => x.RefreshToken == refreshToken,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    public async Task<User?> GetByResetTokenAsync(
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.Users
            .Include(x => x.Organization)
            .FirstOrDefaultAsync(
                x => x.ResetPasswordToken == resetToken,
                cancellationToken);

        return model == null
            ? null
            : ToEntity(model);
    }

    private static User ToEntity(
        UserDataModel model)
    {
        var organization = model.Organization == null ? null : new Organization(
            model.Organization.Id,
            model.Organization.OrganizationTypeId,
            model.Organization.Name,
            model.Organization.Address,
            model.Organization.Status,
            model.Organization.CreatedAt,
            model.Organization.UpdatedAt,
            null);

        return User.Rehydrate(
            model.Id,
            model.OrganizationId,
            model.FullName,
            model.Email,
            model.PasswordHash!,
            model.Phone,
            model.Role,
            model.IsActive,
            model.CreatedAt,
            model.UpdatedAt,
            model.RefreshToken,
            model.RefreshTokenExpiry,
            model.ResetPasswordToken,
            model.ResetPasswordTokenExpiry,
            organization);
    }





    private static UserDataModel ToModel(
        User entity)
    {

        return new UserDataModel
        {

            Id = entity.Id,


            OrganizationId =
                entity.OrganizationId,


            Email =
                entity.Email,


            PasswordHash =
                entity.PasswordHash,


            FullName =
                entity.FullName,


            Phone =
                entity.Phone,


            Role =
                entity.Role,


            IsActive =
                entity.IsActive,


            RefreshToken =
                entity.RefreshToken,


            RefreshTokenExpiry =
                entity.RefreshTokenExpiry,


            ResetPasswordToken =
                entity.ResetPasswordToken,


            ResetPasswordTokenExpiry =
                entity.ResetPasswordTokenExpiry,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}
