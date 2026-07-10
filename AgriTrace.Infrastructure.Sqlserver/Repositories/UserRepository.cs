using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities;
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
            .OrderBy(x => x.Email)
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
            .AsQueryable();



        var totalCount = await query
            .CountAsync(cancellationToken);



        var models = await query
            .OrderBy(x => x.Email)
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



        model.OrganizationId =
            entity.OrganizationId;


        model.Email =
            entity.Email;


        model.FullName =
            entity.FullName;


        model.Role =
            entity.Role;


        model.IsActive =
            entity.IsActive;


        model.UpdatedAt =
            DateTime.UtcNow;



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
            .Where(
                x => x.OrganizationId == organizationId)
            .OrderBy(
                x => x.Email)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    public async Task<IReadOnlyList<User>> GetByRoleAsync(
        string role,
        CancellationToken cancellationToken = default)
    {

        var models = await _context.Users
            .Where(
                x => x.Role == role)
            .OrderBy(
                x => x.Email)
            .ToListAsync(cancellationToken);



        return models
            .Select(ToEntity)
            .ToList();

    }





    private static User ToEntity(
        UserDataModel model)
    {

        return new User(
            model.OrganizationId,
            model.Email,
            model.PasswordHash,
            model.FullName,
            model.Role);

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


            Role =
                entity.Role,


            IsActive =
                entity.IsActive,


            CreatedAt =
                entity.CreatedAt,


            UpdatedAt =
                entity.UpdatedAt

        };

    }

}