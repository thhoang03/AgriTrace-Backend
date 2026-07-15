using AgriTrace.API.Models;
using AgriTrace.Application.Features.Certificates.Commands;
using AgriTrace.Application.Features.Certificates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AgriTrace.API.Controllers;

/// <summary>
/// Manages certificates for agricultural batches.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all certificates for a specific batch.
    /// Authorization: Authenticated
    /// </summary>
    [HttpGet("api/v1/batches/{batchId:guid}/certificates")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCertificatesByBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dtos = await _mediator.Send(
            new GetCertificatesByBatchQuery(batchId),
            cancellationToken);

        var response = dtos.Select(ToResponse).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Issue a new certificate for a batch.
    /// Authorization: INSPECTOR
    /// </summary>
    [HttpPost("api/v1/batches/{batchId:guid}/certificates")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
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
    /// Get the details of a specific certificate.
    /// Authorization: Authenticated
    /// </summary>
    [HttpGet("api/v1/certificates/{certificateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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
                ApiResponse.Fail(
                    System.Net.HttpStatusCode.NotFound,
                    $"Certificate '{certificateId}' was not found."));
        }

        return Ok(ToResponse(dto));
    }

    /// <summary>
    /// Revoke an existing certificate.
    /// Authorization: ADMIN
    /// </summary>
    [HttpDelete("api/v1/certificates/{certificateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
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
            IssuedDate = dto.IssuedDate,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}
