using AgriTrace.Domain.Common;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Interfaces.Outbound;
using AgriTrace.Infrastructure.Sqlserver.Models;
using AgriTrace.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AgriTrace.Infrastructure.Sqlserver.Repositories;

public class QualityInspectionRepository
    : IQualityInspectionRepository
{
    private readonly ApplicationDbContext _context;

    public QualityInspectionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // ── Standard CRUD ──

    public async Task<QualityInspection> AddAsync(
        QualityInspection entity,
        CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await _context.QualityInspections
            .AddAsync(model, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.QualityInspections
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (model == null) return;

        _context.QualityInspections.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<QualityInspection>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var models = await _context.QualityInspections
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<QualityInspection?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.QualityInspections
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntity(model);
    }

    public async Task<PagedResult<QualityInspection>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.QualityInspections
            .Include(x => x.Batch)
            .Include(x => x.Inspector)
            .Include(x => x.LabTests)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<QualityInspection>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task UpdateAsync(
        QualityInspection entity,
        CancellationToken cancellationToken = default)
    {
        var trackedTestIds = _context.ChangeTracker
            .Entries<InspectionLabTestDataModel>()
            .Where(e => e.Entity.InspectionId == entity.Id)
            .Select(e => e.Entity.Id)
            .ToHashSet();

        foreach (var entry in _context.ChangeTracker.Entries<InspectionLabTestDataModel>().ToList())
        {
            if (trackedTestIds.Contains(entry.Entity.Id))
            {
                entry.State = EntityState.Detached;
            }
        }

        var tracked = _context.ChangeTracker
            .Entries<QualityInspectionDataModel>()
            .FirstOrDefault(e => e.Entity.Id == entity.Id);

        if (tracked != null)
        {
            tracked.State = EntityState.Detached;
        }

        var model = await _context.QualityInspections
            .Include(x => x.LabTests)
            .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (model == null) return;

        model.InspectionType = entity.InspectionType;
        model.Status = entity.Status;
        model.OverallResult = entity.OverallResult;
        model.InspectionDate = entity.InspectionDate;
        model.Notes = entity.Notes;
        model.UpdatedAt = DateTime.UtcNow;

        // Sync lab tests
        var existingTests = model.LabTests.ToDictionary(t => t.Id);
        var updatedTestIds = entity.LabTests.Select(t => t.Id).ToHashSet();

        foreach (var test in entity.LabTests)
        {
            if (existingTests.TryGetValue(test.Id, out var existingTest))
            {
                existingTest.TestName = test.TestName;
                existingTest.MeasuredValue = test.MeasuredValue;
                existingTest.Unit = test.Unit;
                existingTest.MinStandardValue = test.MinStandardValue;
                existingTest.MaxStandardValue = test.MaxStandardValue;
                existingTest.IsPassed = test.IsPassed;
                existingTest.Remark = test.Remark;
            }
            else
            {
                model.LabTests.Add(ToLabTestModel(test));
            }
        }

        foreach (var existingTest in model.LabTests.ToList())
        {
            if (!updatedTestIds.Contains(existingTest.Id))
            {
                _context.InspectionLabTests.Remove(existingTest);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    // ── Custom Queries ──

    public async Task<IReadOnlyList<QualityInspection>> GetByBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.QualityInspections
            .Include(x => x.Batch)
            .Include(x => x.Inspector)
            .Include(x => x.LabTests)
            .Where(x => x.BatchId == batchId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<IReadOnlyList<QualityInspection>> GetByInspectorAsync(
        Guid inspectorId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.QualityInspections
            .Where(x => x.InspectorId == inspectorId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToEntity).ToList();
    }

    public async Task<PagedResult<QualityInspection>> GetPagedByOrganizationAsync(
        Guid? organizationId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.QualityInspections
            .Include(x => x.Batch)
            .Include(x => x.Inspector)
            .Include(x => x.LabTests)
            .AsQueryable();

        if (organizationId.HasValue)
        {
            query = query.Where(x => x.OrganizationId == organizationId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var models = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<QualityInspection>(
            models.Select(ToEntity).ToList(),
            totalCount,
            pageNumber,
            pageSize);
    }

    public async Task<QualityInspection?> GetByIdWithLabTestsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.QualityInspections
            .Include(x => x.Batch)
            .Include(x => x.Inspector)
            .Include(x => x.LabTests)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return model == null ? null : ToEntityWithLabTests(model);
    }

    // ── Lab Test Direct Operations ──

    public async Task<IReadOnlyList<InspectionLabTest>> GetLabTestsAsync(
        Guid inspectionId,
        CancellationToken cancellationToken = default)
    {
        var models = await _context.InspectionLabTests
            .Where(x => x.InspectionId == inspectionId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return models.Select(ToLabTestEntity).ToList();
    }

    public async Task AddLabTestAsync(
        InspectionLabTest labTest,
        CancellationToken cancellationToken = default)
    {
        var model = ToLabTestModel(labTest);
        await _context.InspectionLabTests.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateLabTestAsync(
        InspectionLabTest labTest,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.InspectionLabTests
            .FirstOrDefaultAsync(x => x.Id == labTest.Id, cancellationToken);

        if (model == null) return;

        model.TestName = labTest.TestName;
        model.MeasuredValue = labTest.MeasuredValue;
        model.Unit = labTest.Unit;
        model.MinStandardValue = labTest.MinStandardValue;
        model.MaxStandardValue = labTest.MaxStandardValue;
        model.IsPassed = labTest.IsPassed;
        model.Remark = labTest.Remark;
        model.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteLabTestAsync(
        Guid labTestId,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.InspectionLabTests
            .FirstOrDefaultAsync(x => x.Id == labTestId, cancellationToken);

        if (model == null) return;

        _context.InspectionLabTests.Remove(model);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<InspectionLabTest?> GetLabTestByIdAsync(
        Guid labTestId,
        CancellationToken cancellationToken = default)
    {
        var model = await _context.InspectionLabTests
            .FirstOrDefaultAsync(x => x.Id == labTestId, cancellationToken);

        return model == null ? null : ToLabTestEntity(model);
    }

    public async Task SaveLabTestsAsync(
        Guid inspectionId,
        IReadOnlyList<InspectionLabTest> labTests,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.InspectionLabTests
            .Where(x => x.InspectionId == inspectionId)
            .ToListAsync(cancellationToken);

        _context.InspectionLabTests.RemoveRange(existing);

        var models = labTests.Select(ToLabTestModel).ToList();
        await _context.InspectionLabTests.AddRangeAsync(models, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // ── Mapping ──

    private static QualityInspection ToEntity(QualityInspectionDataModel model)
    {
        return new QualityInspection(
            model.Id,
            model.BatchId,
            model.OrganizationId,
            model.InspectorId,
            model.InspectionType,
            model.Status,
            model.OverallResult,
            model.InspectionDate,
            model.Notes,
            model.CreatedAt,
            model.UpdatedAt,
            model.LabTests.Select(ToLabTestEntity).ToList(),
            ToBatchEntity(model.Batch),
            ToUserEntity(model.Inspector));
    }

    private static Batch? ToBatchEntity(BatchDataModel? model)
    {
        if (model is null) return null;
        return Batch.Rehydrate(
            model.Id, model.ProductId, model.BatchCode, model.Quantity,
            model.RemainingQuantity, model.SourceQuantity, model.UnitId,
            model.ProductionDate, model.ExpiryDate, model.Status,
            model.CurrentOrganizationId, model.QRCode, model.ParentBatchId,
            model.RootBatchId, model.SplitId, model.CreatedAt, model.UpdatedAt);
    }

    private static User? ToUserEntity(UserDataModel? model)
    {
        if (model is null) return null;
        return User.Rehydrate(
            model.Id, model.OrganizationId, model.FullName, model.Email,
            model.PasswordHash ?? string.Empty, model.Phone, model.Role,
            model.IsActive, model.Status, model.MustChangePassword,
            model.CreatedAt, model.UpdatedAt, model.RefreshToken,
            model.RefreshTokenExpiry, model.ResetPasswordToken,
            model.ResetPasswordTokenExpiry, null);
    }

    private static QualityInspection ToEntityWithLabTests(QualityInspectionDataModel model)
    {
        return new QualityInspection(
            model.Id,
            model.BatchId,
            model.OrganizationId,
            model.InspectorId,
            model.InspectionType,
            model.Status,
            model.OverallResult,
            model.InspectionDate,
            model.Notes,
            model.CreatedAt,
            model.UpdatedAt,
            model.LabTests?.Select(ToLabTestEntity).ToList(),
            ToBatchEntity(model.Batch),
            ToUserEntity(model.Inspector));
    }

    private static QualityInspectionDataModel ToModel(QualityInspection entity)
    {
        return new QualityInspectionDataModel
        {
            Id = entity.Id,
            BatchId = entity.BatchId,
            OrganizationId = entity.OrganizationId,
            InspectorId = entity.InspectorId,
            InspectionType = entity.InspectionType,
            Status = entity.Status,
            OverallResult = entity.OverallResult,
            InspectionDate = entity.InspectionDate,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static InspectionLabTest ToLabTestEntity(InspectionLabTestDataModel model)
    {
        return new InspectionLabTest(
            model.Id,
            model.InspectionId,
            model.TestName,
            model.MeasuredValue,
            model.Unit,
            model.MinStandardValue,
            model.MaxStandardValue,
            model.IsPassed,
            model.Remark,
            model.CreatedAt,
            model.UpdatedAt);
    }

    private static InspectionLabTestDataModel ToLabTestModel(InspectionLabTest entity)
    {
        return new InspectionLabTestDataModel
        {
            Id = entity.Id,
            InspectionId = entity.InspectionId,
            TestName = entity.TestName,
            MeasuredValue = entity.MeasuredValue,
            Unit = entity.Unit,
            MinStandardValue = entity.MinStandardValue,
            MaxStandardValue = entity.MaxStandardValue,
            IsPassed = entity.IsPassed,
            Remark = entity.Remark,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
