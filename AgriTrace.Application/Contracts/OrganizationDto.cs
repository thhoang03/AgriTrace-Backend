using AgriTrace.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgriTrace.Application.Contracts
{
    public class OrganizationDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Address { get; init; }

        public OrganizationStatus Status { get; init; }

        public Guid OrganizationTypeId { get; init; }

        public string? OrganizationTypeName { get; init; }

        public string? OrganizationTypeCode { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
