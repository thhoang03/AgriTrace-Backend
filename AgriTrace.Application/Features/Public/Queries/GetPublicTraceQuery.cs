using AgriTrace.Application.Common.Exceptions;
using AgriTrace.Application.Contracts;
using AgriTrace.Domain.Interfaces.Inbound;
using MediatR;

namespace AgriTrace.Application.Features.Public.Queries;

public record GetPublicTraceQuery(Guid BatchId) : IRequest<PublicTraceDto>;

public class GetPublicTraceQueryHandler : IRequestHandler<GetPublicTraceQuery, PublicTraceDto>
{
    private readonly IBatchReadService _batchReadService;
    private readonly IProductReadService _productReadService;
    private readonly IUnitService _unitService;
    private readonly IOrganizationService _organizationService;
    private readonly IEventService _eventService;
    private readonly IEventTypeService _eventTypeService;
    private readonly IQualityInspectionService _inspectionService;
    private readonly ICertificateService _certificateService;
    private readonly IUserService _userService;
    private readonly IRecallService _recallService;

    public GetPublicTraceQueryHandler(
        IBatchReadService batchReadService,
        IProductReadService productReadService,
        IUnitService unitService,
        IOrganizationService organizationService,
        IEventService eventService,
        IEventTypeService eventTypeService,
        IQualityInspectionService inspectionService,
        ICertificateService certificateService,
        IUserService userService,
        IRecallService recallService)
    {
        _batchReadService = batchReadService;
        _productReadService = productReadService;
        _unitService = unitService;
        _organizationService = organizationService;
        _eventService = eventService;
        _eventTypeService = eventTypeService;
        _inspectionService = inspectionService;
        _certificateService = certificateService;
        _userService = userService;
        _recallService = recallService;
    }

    public async Task<PublicTraceDto> Handle(GetPublicTraceQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchReadService.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new NotFoundException($"Batch {request.BatchId} not found.");

        var product = await _productReadService.GetByIdAsync(batch.ProductId, cancellationToken);
        var unit = await _unitService.GetByIdAsync(batch.UnitId, cancellationToken);
        var organization = await _organizationService.GetByIdAsync(batch.CurrentOrganizationId, cancellationToken);

        // Timeline from supply-chain events (ordered by time).
        var events = await _eventService.GetByBatchAsync(batch.Id, cancellationToken);

        var eventTypeCodes = new Dictionary<Guid, string?>();
        var orgNames = new Dictionary<Guid, string?>();
        var timeline = new List<TimelineEventDto>();
        foreach (var ev in events.OrderBy(e => e.EventTime))
        {
            if (!eventTypeCodes.TryGetValue(ev.EventTypeId, out var code))
            {
                var type = await _eventTypeService.GetByIdAsync(ev.EventTypeId, cancellationToken);
                code = type?.Code;
                eventTypeCodes[ev.EventTypeId] = code;
            }

            if (!orgNames.TryGetValue(ev.OrganizationId, out var orgName))
            {
                var org = await _organizationService.GetByIdAsync(ev.OrganizationId, cancellationToken);
                orgName = org?.Name;
                orgNames[ev.OrganizationId] = orgName;
            }

            timeline.Add(new TimelineEventDto
            {
                EventTypeCode = code,
                OrganizationName = orgName,
                EventTime = ev.EventTime,
                Location = ev.Location
            });
        }

        // Inspections.
        var inspections = await _inspectionService.GetByBatchAsync(batch.Id, cancellationToken);
        var inspectionSummaries = new List<PublicInspectionDto>();
        foreach (var inspection in inspections.OrderBy(i => i.CreatedAt))
        {
            var inspector = await _userService.GetByIdAsync(inspection.InspectorId, cancellationToken);
            inspectionSummaries.Add(new PublicInspectionDto
            {
                Result = inspection.Result,
                InspectorName = inspector?.FullName,
                CreatedAt = inspection.CreatedAt
            });
        }

        // Certificates.
        var certificates = await _certificateService.GetByBatchAsync(batch.Id, cancellationToken);
        var certificateSummaries = certificates
            .OrderBy(c => c.CreatedAt)
            .Select(c => new PublicCertificateDto
            {
                CertificateType = c.CertificateType,
                FileUrl = c.FileUrl,
                IssuedDate = c.IssuedDate
            })
            .ToList();

        // Recall status (latest recall for the batch, if any).
        var recalls = await _recallService.GetByBatchAsync(batch.Id, cancellationToken);
        var latestRecall = recalls.OrderByDescending(r => r.CreatedAt).FirstOrDefault();

        return new PublicTraceDto
        {
            BatchId = batch.Id,
            BatchCode = batch.BatchCode,
            ProductName = product?.Name,
            Quantity = batch.Quantity,
            UnitCode = unit?.Code,
            CurrentOrganizationName = organization?.Name,
            Status = (int)batch.Status,
            Timeline = timeline,
            Inspections = inspectionSummaries,
            Certificates = certificateSummaries,
            RecallStatus = latestRecall?.Status.ToString().ToUpperInvariant()
        };
    }
}
