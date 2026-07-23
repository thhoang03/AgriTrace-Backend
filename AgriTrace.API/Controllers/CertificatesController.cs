using AgriTrace.API.Models;
using AgriTrace.Application.Features.Certificates.Commands;
using AgriTrace.Application.Features.Certificates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Manages certificates for agricultural batches.
/// </summary>
[ApiController]
[Authorize]
[Produces("application/json")]
public sealed class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Danh sách chứng nhận của Batch
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/certificates")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCertificatesByBatch(
        Guid batchId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var dtos = await _mediator.Send(
            new GetCertificatesByBatchQuery(batchId),
            cancellationToken);

        var all = dtos.Select(ToResponse).ToList();

        var pageItems = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var response = new CertificatePagedResponse(
            pageItems,
            all.Count,
            page,
            pageSize);

        return Ok(response);
    }

    /// <summary>
    /// Cấp chứng nhận
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/certificates")]
    [Authorize(Roles = "Inspector,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IssueCertificate(
        Guid batchId,
        [FromBody] IssueCertificateRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new IssueCertificateCommand(
                batchId,
                request.InspectionId,
                request.CertificateType,
                request.FileUrl,
                request.IssuedDate),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetCertificateById),
            new { certificateId = dto.Id },
            new { certificateId = dto.Id });
    }

    /// <summary>
    /// Chi tiết chứng nhận
    /// </summary>
    [HttpGet("api/v1/certificates/{certificateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCertificateById(
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(
            new GetCertificateByIdQuery(certificateId),
            cancellationToken);

        if (dto is null)
        {
            return NotFound(
                ErrorResponse.Fail(
                    $"Certificate '{certificateId}' was not found."));
        }

        return Ok(ToResponse(dto));
    }

    /// <summary>
    /// Thu hồi chứng nhận
    /// </summary>
    [HttpDelete("api/v1/certificates/{certificateId:guid}")]
    [Authorize(Roles = "Inspector,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeCertificate(
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RevokeCertificateCommand(certificateId),
            cancellationToken);

        return NoContent();
    }

    private static CertificateResponse ToResponse(
        AgriTrace.Application.Contracts.CertificateDto dto)
    {
        return new CertificateResponse
        {
            CertificateId = dto.Id,
            BatchId = dto.BatchId,
            BatchCode = dto.BatchCode,
            InspectionId = dto.InspectionId,
            CertificateType = dto.CertificateType,
            FileUrl = dto.FileUrl,
            IssuedDate = dto.IssuedDate.HasValue ? DateOnly.FromDateTime(dto.IssuedDate.Value) : null,
            CreatedAt = dto.CreatedAt
        };
    }
}
