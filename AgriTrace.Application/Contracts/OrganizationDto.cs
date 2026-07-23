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

