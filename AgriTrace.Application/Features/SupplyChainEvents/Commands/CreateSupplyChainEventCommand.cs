using AgriTrace.Application.Contracts;
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
using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Features.SupplyChainEvents.Commands;

/// <summary>
/// Command to record a new supply chain event for a batch.
/// The hash chain (previousHash / currentHash) is computed automatically by the domain service.
/// </summary>
public sealed record CreateSupplyChainEventCommand(
    Guid BatchId,
    Guid EventTypeId,
    Guid OrganizationId,
    Guid PerformedByUserId,
    string? EventData,
    string? Location)
    : IRequest<SupplyChainEventDto>;

public sealed class CreateSupplyChainEventCommandHandler
    : IRequestHandler<CreateSupplyChainEventCommand, SupplyChainEventDto>
{
    private readonly ISupplyChainEventWriteService _writeService;

    public CreateSupplyChainEventCommandHandler(ISupplyChainEventWriteService writeService)
    {
        _writeService = writeService;
    }

    public async Task<SupplyChainEventDto> Handle(
        CreateSupplyChainEventCommand command,
        CancellationToken cancellationToken)
    {
        var entity = new SupplyChainEvent(
            command.BatchId,
            command.EventTypeId,
            command.OrganizationId,
            command.PerformedByUserId,
            command.EventData,
            command.Location,
            previousHash: null,
            currentHash: null);

        var created = await _writeService.CreateAsync(entity, cancellationToken);

        return ToDto(created);
    }

    internal static SupplyChainEventDto ToDto(SupplyChainEvent e) => new()
    {
        Id = e.Id,
        BatchId = e.BatchId,
        EventTypeId = e.EventTypeId,
        OrganizationId = e.OrganizationId,
        PerformedByUserId = e.PerformedByUserId,
        EventData = e.EventData,
        Location = e.Location,
        PreviousHash = e.PreviousHash,
        CurrentHash = e.CurrentHash,
        EventTime = e.EventTime,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}

public sealed class CreateSupplyChainEventCommandValidator
    : AbstractValidator<CreateSupplyChainEventCommand>
{
    public CreateSupplyChainEventCommandValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithMessage("BatchId is required.");

        RuleFor(x => x.EventTypeId)
            .NotEmpty()
            .WithMessage("EventTypeId is required.");

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("OrganizationId is required.");

        RuleFor(x => x.PerformedByUserId)
            .NotEmpty()
            .WithMessage("PerformedByUserId is required.");

        RuleFor(x => x.EventData)
            .MaximumLength(4000)
            .When(x => x.EventData != null)
            .WithMessage("EventData must not exceed 4000 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(500)
            .When(x => x.Location != null)
            .WithMessage("Location must not exceed 500 characters.");
    }
}

